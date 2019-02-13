import { Application, RpcClass } from "mydog";
import { isTokenLegal } from "../../../domain/login";

export default function (app: Application) {
    return new Remote(app);
}

declare global {
    interface Rpc {
        gate: {
            main: RpcClass<Remote>
        }
    }
}

class Remote {
    app: Application;
    constructor(app: Application) {
        this.app = app;
    }
    isTokenOK(uid: number, token: number, cb: Function) {
        isTokenLegal(uid, token, cb);
    };


}
