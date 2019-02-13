import mysql = require("mysql");

export default class mysqlClient {
    private config: any;
    private pool: any;
    constructor(config: any) {
        this.config = config;
        this.pool = mysql.createPool({
            host: config['host'],
            port: config['port'],
            user: config['user'],
            password: config['password'],
            database: config['database'],
            connectionLimit: config['limit']
        });
    }

    /**
     * 执行mysql语句
     * @param sql sql语句
     * @param args sql参数
     * @param cb 回调
     */
    query(sql: string, args?: any[], cb?: Function) {
        this.pool.getConnection(function (err: any, connection: any) {
            if (!err) {
                connection.query(sql, args, function (err: any, res: any) {
                    connection.release();
                    cb && cb(err, res);
                });
            } else {
                cb && cb(err);
            }
        });
    }

    /**
     * mysql 查询
     * @param table 表名
     * @param field 字段数组
     * @param objCon 条件
     * @param cb 回调
     */
    select(table: string, field: "*" | string[], objCon: normalObj, cb: Function) {
        let sql = "select ";
        if (field === "*") {
            sql += field;
        } else {
            sql += field.join();
        }
        sql += " from " + table + " where ";
        let whereStr = "";
        let value: any;
        for (let key in objCon) {
            value = objCon[key];
            if (typeof value === "string") {
                whereStr += key + "='" + value + "' and ";
            } else {
                whereStr += key + "=" + value + " and ";
            }
        }
        sql += whereStr.substring(0, whereStr.length - 5);
        this.query(sql, undefined, cb);
    }

    /**
     * mysql 插入
     * @param table 表名
     * @param obj 数据
     * @param cb 回调
     */
    insert(table: string, obj: normalObj, cb?: Function) {
        let sql = "insert into " + table + "(";
        let fieldStr = "";
        let valueStr = "";
        let value;
        for (let key in obj) {
            value = obj[key];
            fieldStr += key + ",";
            if (typeof value === "string") {
                valueStr += "'" + value + "',";
            } else {
                valueStr += value + ",";
            }
        }
        sql += fieldStr.substring(0, fieldStr.length - 1) + ") values(" + valueStr.substring(0, valueStr.length - 1) + ")";
        this.query(sql, undefined, cb);
    }

    /**
     * mysql 更新
     * @param table 表名
     * @param obj 数据
     * @param objCon 条件
     * @param cb 回调
     */
    update(table: string, obj: normalObj, objCon: normalObj, cb?: Function) {
        let sql = "update " + table + " set ";
        let updateStr = "";
        let value;
        for (let key in obj) {
            value = obj[key];
            if (typeof value === "string") {
                updateStr += key + "='" + value + "',";
            } else {
                updateStr += key + "=" + value + ",";
            }
        }
        let whereStr = " where ";
        for (let key in objCon) {
            value = objCon[key];
            if (typeof value === "string") {
                whereStr += key + "='" + value + "' and ";
            } else {
                whereStr += key + "=" + value + " and ";
            }
        }
        sql += updateStr.substring(0, updateStr.length - 1) + whereStr.substring(0, whereStr.length - 5);
        this.query(sql, undefined, cb);
    }

    /**
     * mysql 删除
     * @param table 表名
     * @param objCon 条件
     * @param cb 回调
     */
    delete(table: string, objCon: normalObj, cb?: Function) {
        let sql = "delete from " + table + " where ";
        let key;
        let value;
        let whereStr = "";
        for (key in objCon) {
            value = objCon[key];
            whereStr += key + "=";
            if (typeof value === "string") {
                whereStr += "'" + value + "' and ";
            } else {
                whereStr += value + " and ";
            }
        }
        sql += whereStr.substring(0, whereStr.length - 5);
        this.query(sql, undefined, cb);
    }
}