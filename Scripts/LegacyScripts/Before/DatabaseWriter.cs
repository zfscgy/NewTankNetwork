using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DatabaseWriter : MonoBehaviour {
    public DataMgr manager;
    public List<TankControl> AllTanks;
    public MainGameController topController;
    //
    bool useDb;
    //记录鼠标偏移量的状态类
    ZF.TankOrder deltaAngles;
    //记录上一次鼠标相对坦克角度用于计算偏移量
    Vector3 lastMove=new Vector3(0,0,0);
    // 用于控制数据库记录的时间
    float statusTime;
    float operationTime;
    // Use this for initialization
    void Start ()
    {
        //这里控制是否使用数据库
        useDb = false;
        // 初始化计时时间
        statusTime = Time.time;
        operationTime = Time.time;
        AllTanks = topController.AllPlayers;
        manager = new DataMgr();
        manager.Initial();
        if (useDb && PhotonNetwork.isMasterClient)
        {
            manager.GetGameId();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (useDb)
        {
            // 这里改下，只写入自己的数据
            foreach (ZF.IFetchTankInfo tank in AllTanks)
            {
                WriteData(tank);
            }
        }
	}

    private void WriteData(ZF.IFetchTankInfo tank)
    {
        ZF.TankInfo tankInfo = tank.FetchTankInfo();
        // 记录鼠标指向的方向是否有位移
        // 计算鼠标偏移量
        CalDeri(tankInfo);
        // 间隔时间大于0.1s可以记录一次状态
        if (Time.time - statusTime > 0.06)
        {
            manager.StoreStatus(tankInfo);
            statusTime = Time.time;
        }
        // 间隔时间大于0.02s且存在有效操作时记录操作
        if (Time.time - operationTime > 0.02 && CanRecOpera(deltaAngles))
        {
            manager.StoreOperation(deltaAngles);
            operationTime = Time.time;
        }
    }
    //计算两次鼠标与坦克的相对方向的偏移量
    private void CalDeri(ZF.TankInfo _tankInfo)
    {
        // 复制类
        deltaAngles.move = _tankInfo.tankOrder.move;
        deltaAngles.steer = _tankInfo.tankOrder.steer;
        deltaAngles.shoot = _tankInfo.tankOrder.shoot;
        //计算与上一次的偏差
        deltaAngles.direction.x = _tankInfo.tankOrder.direction.x - lastMove.x;
        deltaAngles.direction.y = _tankInfo.tankOrder.direction.y - lastMove.y;
        deltaAngles.direction.z = _tankInfo.tankOrder.direction.z - lastMove.z;
        //更新上一次相对坦克方向
        lastMove.x = _tankInfo.tankOrder.direction.x;
        lastMove.y = _tankInfo.tankOrder.direction.y;
        lastMove.z = _tankInfo.tankOrder.direction.z;
    }
    // 是否存在有效操作
    private bool CanRecOpera(ZF.TankOrder _order)
    {
        // 是否在前进/转弯/开火
        if (_order.move != 0 || _order.steer != 0 || _order.shoot != 0)
        {
            return true;
        }
        // 是否准心发生了移动
        if (Math.Abs(_order.direction.x) > 0.1f || Math.Abs(_order.direction.y) > 0.1f || Math.Abs(_order.direction.z) > 0.1f)
        {
            return true;
        }
        return false;
    }
}
