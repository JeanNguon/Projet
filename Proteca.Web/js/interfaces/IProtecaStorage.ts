module proteca {
    export interface IProtecaStorage {
        get(): ProtecaItem[];
        put(proteca: ProtecaItem[]);
    }
}