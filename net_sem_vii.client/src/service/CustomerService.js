export const CustomerService = {
    getData() {
        return [
            {
                id: 1000,
                name: 'James Butt',
                date: '2015-09-13',
                type: 
                {
                    name: 'DamperPosition'
                },
                value: 70663
            },
            {
                id: 1001,
                name: 'Josephine Darakjy',
                date: '2019-02-09',
                activity: 0,
                type: {
                    name: 'WheelTemperature',
                },
                value: 82429
            },
            {
                id: 1002,
                name: 'Art Venere',
                date: '2017-05-13',
                activity: 63,
                type: {
                    name: 'WheelSpeed',
                },
                value: 28334
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
