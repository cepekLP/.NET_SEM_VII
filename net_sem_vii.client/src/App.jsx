import { useEffect, useState, useRef, useMemo } from 'react';
import useWebSocket from 'react-use-websocket';
import { DataTable } from 'primereact/datatable';
import { InputText } from 'primereact/inputtext'
import { Column } from 'primereact/column';

import { FilterMatchMode, FilterOperator } from 'primereact/api';

import { InputNumber } from 'primereact/inputnumber';
import { Button } from 'primereact/button';

import { Calendar } from 'primereact/calendar';
import { MultiSelect } from 'primereact/multiselect';

import { Chart } from 'primereact/chart';
import 'primereact/resources/themes/lara-light-indigo/theme.css';
import './App.css';


const WS_URL = 'wss://localhost:6969/ws';

function App() {

    const dt = useRef(null);
    const [customers, setCustomers] = useState(null);
    const [accdata, setAccData] = useState(null);
    const [filters, setFilters] = useState(null);
    const [loading] = useState(false);
    const [dynamicDesktop, setDynDesk] = useState(false);

    const [globalFilterValue, setGlobalFilterValue] = useState('');

    const queryParameters = new URLSearchParams(window.location.search);
    const dynamicDesktopQuery = queryParameters.has("dynamicDesktop");


    const [types] = useState([
        'RideHeight',
        'WheelSpeed',
        'WheelTemperature',
        'DamperPosition'
    ]);

    function checkDynamicDesktop() {
        setDynDesk(dynamicDesktopQuery);
    }


    //CHART PART
    const [chartData, setChartData] = useState({});
    const [chartOptions, setChartOptions] = useState({});

    const [newData, setNewData] = useState({});



    const [isFirst, setIsFirst] = useState(true);

    useWebSocket(WS_URL, {
        onOpen: () => {
            console.log('WebSocket connection established.');
        },
        onMessage: (event) => {
            if (dynamicDesktopQuery) {
                try {

                    if (isFirst) {
                        setIsFirst(false);
                        //console.log(event.data);
                        const newData = JSON.parse(event.data);
                        console.log(newData);
                        console.log(isFirst);
                        setCustomers(newData.map((d) => {
                            d.Date = new Date(d.Date);
                            return d;
                        }));


                    }
                    else {
                        const newData = JSON.parse(event.data);
                        console.log(newData);
                        setAccData(newData);
                    }

                    //setCustomers()
                } catch (err) {
                    console.log(err);
                }
            }
        }
    });


    const documentStyle = getComputedStyle(document.documentElement);

    const data = {
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July'],
        datasets: [
            {
                label: 'First Dataset',
                data: [65, 59, 80, 81, 56, 55, 40],
                fill: false,
                borderColor: documentStyle.getPropertyValue('--blue-500'),
                tension: 0.4
            },
            {
                label: 'Second Dataset',
                data: [28, 48, 40, 19, 86, 27, 90],
                fill: false,
                borderColor: documentStyle.getPropertyValue('--pink-500'),
                tension: 0.4
            }
        ]
    };
    const fetchAndSetData = (filter) => {
        fetch("/sensorData?" + filter).then((de) => {
            de.json().then((a) => {
                console.log(a)
                setCustomers(a);
                setChartData(buildData(a))
            })

        }).catch((err) => console.log(err))
    }
    const [typeFilters, setTypeFilters] = useState();
    const [idFilters, setIdFilters] = useState();
    const [dateFilters, setDateFilters] = useState();
    useEffect(() => {

        checkDynamicDesktop();
        if (dynamicDesktop) {
            fetchAndSetData();
        }

        initFilters();

        //CHART VALUES

        const textColor = documentStyle.getPropertyValue('--text-color');
        const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
        const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

        const options = {
            maintainAspectRatio: false,
            aspectRatio: 0.6,
            plugins: {
                legend: {
                    labels: {
                        color: textColor
                    }
                }
            },
            scales: {
                x: {
                    ticks: {
                        color: textColorSecondary
                    },
                    grid: {
                        color: surfaceBorder
                    }
                },
                y: {
                    ticks: {
                        color: textColorSecondary
                    },
                    grid: {
                        color: surfaceBorder
                    }
                }
            }
        };

        setChartData(data);
        setChartOptions(options);
    }, []);

    useEffect(() => {
        let query = "";
        if (typeFilters) {
            query += "type=" + typeFilters + "&";
        }
        if (idFilters) {
            query += "id=" + idFilters + "&";
        }
        if (dateFilters) {
            query += "minDate=" + new Date(dateFilters[0]).toUTCString() + "&";
            if (dateFilters[1])
            query += "maxDate=" + new Date(dateFilters[1]).toUTCString();
        }
        console.log(query)
        fetchAndSetData(query)
    }, [typeFilters, idFilters, dateFilters])

    const clearFilter = () => {
        initFilters();
    };

    const onGlobalFilterChange = (e) => {
        const value = e.target.value;
        let _filters = { ...filters };

        _filters['global'].value = value;
        console.log(_filters);
        setFilters(_filters);
        setGlobalFilterValue(value);
    };

    const initFilters = () => {
        setFilters({
            global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            SensorId: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }] },
            SensorType: { value: null, matchMode: FilterMatchMode.IN },
            Date: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.DATE_IS }] },
            Value: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS }] },
        });
        setGlobalFilterValue('');
    };

    const renderHeader = () => {
        return (
            <div className="flex justify-content-between">
                <Button type="button" icon="pi pi-filter-slash" label="Clear" outlined onClick={clearFilter} />
                <span className="p-input-icon-left">
                    <i className="pi pi-search" />
                    <InputText value={globalFilterValue} onChange={onGlobalFilterChange} placeholder="Keyword Search" />
                </span>
            </div>
        );
    };



    const typeFilterTemplate = (options) => {
        return <MultiSelect value={typeFilters} options={types} itemTemplate={typesItemTemplate} onChange={(e) => { setTypeFilters(e.value); }} optionLabel="name" placeholder="Any" className="w-full md:w-20rem" />;
    };

    const typesItemTemplate = (option) => {
        return (
            <div className="flex align-items-center gap-2">
                <span>{option}</span>
            </div>
        );
    };


    const dateFilterTemplate = (options) => {
        return <Calendar style={{ width: '400px' }} value={dateFilters} onChange={(e) => { setDateFilters(e.value); console.log(e) }} selectionMode="range" showTime hourFormat="24" />;
    };


    const valueFilterTemplate = (options) => {
        return <MultiSelect value={idFilters} options={["0", "1", "2", "3"]} itemTemplate={typesItemTemplate} onChange={(e) => { setIdFilters(e.value); }} optionLabel="name" placeholder="Any" className="w-full md:w-20rem" />;
    };

    ///// CHARTS

    const colors = ["blue", "red", "green", "pink", "cyan", "magenta", "black", "yellow"]

    const getFilteredData = (filteredData) => {
        if (filteredData) {
            return types.flatMap(t => {
                return ["0", "1", "2", "3"].map((n) => {
                    return filteredData.filter(el => el.sensorId === n && el.sensorType === t)
                })
            })
        }
        else {
            return {};
        }

    }

    const buildData = (aData) => {
        const filData2 = getFilteredData(aData.sort((a, b) => { return new Date(a.date) - new Date(b.date); }));
        console.log(filData2);
        const filData = (filData2.filter((el) => el.length != 0));


        if (filData.length) {
            console.log()
            let i = 0;
            const dataset = filData.flatMap((e) => {
                if (e.length) {
                    i++;
                    return {
                        label: e[0].sensorType + e[0].sensorId, data: e.map((d) => d.value), fill: false, borderColor: documentStyle.getPropertyValue('--' + colors[(i - 1) % 8] + '-500'),
                        tension: 0.4
                    }
                }

            });

            if (dataset) {
                const lengths = filData.map(a => a.length);

                const mData = { labels: filData[lengths.indexOf(Math.max(...lengths))].map((a) => { return new Date(a.date).getMinutes().toString() }), datasets: dataset };
                console.log(mData);
                return mData;
            }


        }
        else {
            return null;
        }

    }


    /////// EXPORT

    const exportCSV = (selectionOnly) => {
        dt.current.exportCSV({ selectionOnly });
    };
    const [filtereD, setFilteredD] = useState(customers);
    const exportJSON = () => {
        function download(content, fileName, contentType) {
            var a = document.createElement("a");
            var file = new Blob([content], { type: contentType });
            a.href = URL.createObjectURL(file);
            a.download = fileName;
            a.click();
        }
        download(JSON.stringify(filtereD), 'json.txt', 'text/plain');
    };

    const chartD = useMemo(() => {
        return chartData
    }, [chartData])
    const header = renderHeader();
    return (
        <div className="card" style={{ visibility: false }}>


            {dynamicDesktop && <div>

                <DataTable value={accdata}>
                    <Column field="SensorId" header="SensorId" sortable />
                    <Column field="SensorType" header="SensorType" sortable />
                    <Column field="CurrentValue" header="CurrentValue" sortable />
                    <Column field="AverageValue" header="AverageValue" sortable />
                </DataTable>
            </div>}
            <p></p>
            <p></p>
            
            {customers && dynamicDesktop == false && <div>
                <Chart type="line" data={chartD} options={chartOptions} />
                <Button type="button" icon="pi pi-file" rounded onClick={() => exportCSV(false)} data-pr-tooltip="CSV" >CSV</Button>
                <Button type="button" icon="pi pi-file" rounded onClick={() => exportJSON()} data-pr-tooltip="JSON" >JSON</Button>

                <DataTable paginator rows={100} sortMode="multiple" value={customers} showGridlines loading={loading} dataKey="id" ref={dt} onValueChange={filteredData => {
                    console.log(filteredData);
                    setFilteredD(filteredData)
                    const a = buildData(filteredData); console.log(a); if (a) {
                        setChartData(a);
                    }
                }}
                    filters={filters} header={header} emptyMessage="No entires found.">
                    <Column field="sensorId" header="SensorId" filterField="SensorId" showFilterMatchModes={false} filter filterElement={valueFilterTemplate} sortable />
                    <Column header="SensorType" field="sensorType" filterField="SensorType" showFilterMatchModes={false} showApplyButton={false} filterMenuStyle={{ width: '14rem' }} style={{ minWidth: '14rem' }}
                        filter filterElement={typeFilterTemplate} sortable />
                    <Column header="Date" field="date" filterField="Date" dataType="date" style={{ minWidth: '10rem' }} showApplyButton={false}  showFilterMatchModes={false} filter filterElement={dateFilterTemplate} sortable />
                    <Column header="Value" field="value" filterField="Value" dataType="numeric" style={{ minWidth: '10rem' }} filter filterElement={valueFilterTemplate} sortable />
                </DataTable>
                {/*   <Chart type="line" data={chartData} options={chartOptions} />*/}
            </div>}

        </div>
    );
}

export default App;