class Test {
    private name: string;
    constructor() {
        this.name = "test";
    }

    mymet = function () {
        return this.name;
    };
} 