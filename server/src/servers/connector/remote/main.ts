import { Application, Session, RpcClass } from "mydog";

export default function (app: Application) {
    return new Remote(app);
}

declare global {
    interface Rpc {
        connector: {
            main: RpcClass<Remote>
        }
    }
}

class Remote {
    app: Application;
    constructor(app: Application) {
        this.app = app;
    }
    getUserNum( cb: Function) {
        cb(null, this.app.getBindClientNum());
    };

    kickUser(uid: number){
        this.app.sendMsgByUid("onKicked", null, [uid]);
        this.app.closeClient(uid);
    }
}
