export const CustomerService = {
    getData() {
        return [
            {
                id: 1000,
                SensorId: 'James Butt',
                Date: '2015-09-13',
                type: 'WheelTemperature',
                Value: 70663
            },
            {
                id: 1001,
                SensorId: 'Josephine Darakjy',
                Date: '2019-02-09',
                activity: 0,
                type: 'WheelTemperature',
                Value: 82429
            },
            {
                id: 1002,
                SensorId: 'Art Venere',

                Date: '2017-05-13',
                activity: 63,
                type: 'WheelTemperature',
                Value: 28334
            },
        ];
    },

    getCustomersSmall() {
        return Promise.resolve(this.getData().slice(0, 10));
    },

    getCustomersMedium() {
        return Promise.resolve(this.getData().slice(0, 50));
    },

    getCustomersLarge() {
        return Promise.resolve(this.getData().slice(0, 200));
    },

    getCustomersXLarge() {
        return Promise.resolve(this.getData());
    },

    getCustomers(params) {
        const queryParams = params
            ? Object.keys(params)
                  .map((k) => encodeURIComponent(k) + '=' + encodeURIComponent(params[k]))
                  .join('&')
            : '';

        return fetch('https://www.primefaces.org/data/customers?' + queryParams).then((res) => res.json());
    }
};
