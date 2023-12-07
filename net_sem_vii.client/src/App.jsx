import { useEffect, useState, useRef } from 'react';
import useWebSocket from 'react-use-websocket';
import { DataTable } from 'primereact/datatable';
import { InputText } from 'primereact/inputtext'
import { Column } from 'primereact/column';
import { classNames } from 'primereact/utils';
import { FilterMatchMode, FilterOperator } from 'primereact/api';
import { Dropdown } from 'primereact/dropdown';
import { InputNumber } from 'primereact/inputnumber';
import { Button } from 'primereact/button';
import { ProgressBar } from 'primereact/progressbar';
import { Calendar } from 'primereact/calendar';
import { MultiSelect } from 'primereact/multiselect';
import { Slider } from 'primereact/slider';
import { Tag } from 'primereact/tag';
import { TriStateCheckbox } from 'primereact/tristatecheckbox';
import { CustomerService } from './service/CustomerService';
import { Chart } from 'primereact/chart';
import 'primereact/resources/themes/lara-light-indigo/theme.css';
import './App.css';


const WS_URL = 'wss://localhost:6969/ws';

function App() {

    const dt = useRef(null);
    const [customers, setCustomers] = useState(null);
    const [filters, setFilters] = useState(null);
    const [loading, setLoading] = useState(false);
    const [globalFilterValue, setGlobalFilterValue] = useState('');
    const [types] = useState([
        'RideHeight',
        'WheelSpeed',
        'WheelTemperature',
        'DamperPosition' 
    ]);

    

    //CHART PART
    const [chartData, setChartData] = useState({});
    const [chartOptions, setChartOptions] = useState({});

    const [initialData, setInitialData] = useState({});


    useWebSocket(WS_URL, {
        onOpen: () => {
            console.log('WebSocket connection established.');
        },
        onMessage: (event) => {
            try {
                //console.log(event.data);
                const newData = JSON.parse(event.data);

                setCustomers(newData.map((d) => {
                    d.Date = new Date(d.Date);
                    return d;
                }));
                
                //setCustomers()
            } catch (err) {
                console.log(err);
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

    useEffect(() => {
        // CustomerService.getCustomersMedium().then((data) => {
        //     setCustomers(getCustomers(data));
        //     setLoading(false);
        // });

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

    const getCustomers = (data) => {
        return [...(data || [])].map((d) => {
            d.Date = new Date(d.Date);

            return d;
        });
    };

    const formatDate = (value) => {
        return value.toLocaleDateString('en-US', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    };

    const formatValue = (value) => {
        return value.toString();
    };

    const clearFilter = () => {
        initFilters();
    };

    const onGlobalFilterChange = (e) => {
        const value = e.target.value;
        let _filters = { ...filters };

        _filters['global'].value = value;

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

    const filterClearTemplate = (options) => {
        return <Button type="button" icon="pi pi-times" onClick={options.filterClearCallback} severity="secondary"></Button>;
    };

    const filterApplyTemplate = (options) => {
        return <Button type="button" icon="pi pi-check" onClick={options.filterApplyCallback} severity="success"></Button>;
    };

    const filterFooterTemplate = () => {
        return <div className="px-3 pt-0 pb-3 text-center">Filter by Country</div>;
    };

    const typeBodyTemplate = (rowData) => {
        const type = rowData.SensorType;

        return (
            <div className="flex align-items-center gap-2">
                <span>{type}</span>
            </div>
        );
    };

    const typeFilterTemplate = (options) => {
        return <MultiSelect value={options.value} options={types} itemTemplate={typesItemTemplate} onChange={(e) => options.filterCallback(e.value)} optionLabel="name" placeholder="Any" className="p-column-filter" />;
    };

    const typesItemTemplate = (option) => {
        return (
            <div className="flex align-items-center gap-2">
                <span>{option}</span>
            </div>
        );
    };

    const dateBodyTemplate = (rowData) => {
        return formatDate(rowData.Date);
    };

    const dateFilterTemplate = (options) => {
        return <Calendar value={options.value} onChange={(e) => options.filterCallback(e.value, options.index)} dateFormat="mm/dd/yy" placeholder="mm/dd/yyyy" mask="99/99/9999" />;
    };

    const valueBodyTemplate = (rowData) => {
        return formatValue(rowData.Value);
    };

    const valueFilterTemplate = (options) => {
        return <InputNumber value={options.value} onChange={(e) => options.filterCallback(e.value, options.index)}/>;
    };
 
    const getFilteredData = (filteredData) =>
    {
        if(filteredData)
        {
            return types.map(t => {
                return ["0","1","2","3"].map((n)=> {
                    return filteredData.filter(el =>  el.SensorId === n && el.SensorType === t )
                })
            })
        }
        else
        {
            return {};
        }
        
    }

    const buildData = (aData) => 
    {
        const filData = getFilteredData(aData);
        if(filData[0] && filData[0][0] && filData[0][0][0])
        {
            const dataset = filData.map( (e) => {return {label: e[0][0].SensorType + e[0][0].SensorId, data: e[0].map( (d) => d.Value), fill: false,  borderColor: "red",
            tension: 0.4}});
            
            mData = {labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', '8', '9'], dataset: dataset};
            console.log(mData);
            return mData;
        }
        else
        {
            return {};
        }

    }

    const exportCSV = (selectionOnly) => {
        dt.current.exportCSV({ selectionOnly });
    };


    let newData = null;

    const exportJSON = () => 
    {  
        function download(content, fileName, contentType) {
            var a = document.createElement("a");
            var file = new Blob([content], {type: contentType});
            a.href = URL.createObjectURL(file);
            a.download = fileName;
            a.click();
        }
        download(JSON.stringify(newData), 'json.txt', 'text/plain');
    };

    const header = renderHeader();
    return (
        <div className="card">
            {/* <Chart type="line" data={chartData} options={chartOptions} /> */}

            <div>
                <Button type="button" icon="pi pi-file" rounded onClick={() => exportCSV(false)} data-pr-tooltip="CSV" >CSV</Button>
                <Button type="button" icon="pi pi-file" rounded onClick={() => exportJSON()} data-pr-tooltip="JSON" >JSON</Button>
            </div>
          
            <DataTable  sortMode="multiple" value={customers} showGridlines loading={loading} dataKey="id" ref={dt} onValueChange={filteredData => newData = filteredData}
                    filters={filters} globalFilterFields={['SensorId','SensorType', 'Value']} header={header} emptyMessage="No entires found.">
                <Column field="SensorId" header="SensorId" filter filterPlaceholder="Search by SensorId" style={{ minWidth: '12rem' }} sortable/>
                <Column header="SensorType" field="SensorType" filterField="SensorType" showFilterMatchModes={false} filterMenuStyle={{ width: '14rem' }} style={{ minWidth: '14rem' }}
                    body={typeBodyTemplate} filter filterElement={typeFilterTemplate} sortable/>
                <Column header="Date" field="Date" filterField="Date" dataType="date" style={{ minWidth: '10rem' }} body={dateBodyTemplate} filter filterElement={dateFilterTemplate} sortable/>
                <Column header="Value" field="Value" filterField="Value" dataType="numeric" style={{ minWidth: '10rem' }} body={valueBodyTemplate} filter filterElement={valueFilterTemplate} sortable/>
            </DataTable>

           
        </div>
    );
}

export default App;