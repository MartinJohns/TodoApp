export default class Utils {
    public static uuid(): string {
        /*jshint bitwise:false */
        var i, random;
        var uuid = '';

        for (i = 0; i < 32; i++) {
            random = Math.random() * 16 | 0;
            if (i === 8 || i === 12 || i === 16 || i === 20) {
                uuid += '-';
            }
            uuid += (i === 12 ? 4 : (i === 16 ? (random & 3 | 8) : random))
                .toString(16);
        }

        return uuid;
    }

    public static pluralize(count: number, word: string) {
        return count === 1 ? word : word + 's';
    }

    public static store(key: string, data?: any): any {
        if (data) {
            return localStorage.setItem(key, JSON.stringify(data));
        }

        var store = localStorage.getItem(key);
        return (store && JSON.parse(store)) || [];
    }
}
