export interface Group{
    name: string;
    connections: Connectionn[];
}

interface Connectionn{
    connectionId: string;
    username : string;
}