using System;
using MySql.Data.MySqlClient;
using UnityEngine;
using ZF;

public class DataMgr
{
	MySqlConnection sqlConn = null;
	
	public static DataMgr instance;
	

    //需要存储的数据
    int gameId;
    string connStr = "Database=tankgame;Data Source=127.0.0.1;User Id=game;Password=guest;port=3306";
    public float gameTime;


    public DataMgr()
    {
        instance = this;
        Initial();
    }

    //初始化
    public void Initial()
	{
		
		sqlConn = new MySqlConnection(connStr);
        try
        {
            sqlConn.Open();
            Execute("CREATE TABLE IF NOT EXISTS operation (gameId INT(10), gameTime float, player int, flag int, move int, steer int, derX float, derY float, derZ float, shoot int); ");
            Execute("CREATE TABLE IF NOT EXISTS status (gameId INT(10), gameTime float, player int, flag int, posX float, posY float, posZ float, rotX float, rotY float, rotZ float,  tRotX float, tRotY float, tRotZ float, vX float, vY float, vZ float,hp float);");
            Execute("CREATE TABLE IF NOT EXISTS gameId(gameId INT(10), map int, windX float, windY float, windZ float);");
            Execute("CREATE TABLE IF NOT EXISTS signal(gameId INT(10), gameTime float, player int, flag int, x float, y float, r float, deltaTime float;");
            //pos 感知的敌方位置*15
            //from 感知到的炮弹来源*5
            //bulNum 感知到的敌方剩余弹药量*5
            //hitC 感知到的敌方命中数*5
            //hp 感知到的敌方血量*5
            //turHp 感知到的敌方炮塔血量*5
            //botHp 感知到的敌方底盘血量*5
            Execute("CREATE TABLE IF NOT EXISTS perception(gameId INT(10), gameTime float, player int, flag int, pos1x FLOAT, pos1y FLOAT, pos1z FLOAT, pos2x FLOAT, pos2y FLOAT, pos2z FLOAT, pos3x FLOAT, pos3y FLOAT, pos3z FLOAT, pos4x FLOAT, pos4y FLOAT, pos4z FLOAT, pos5x FLOAT, pos5y FLOAT, pos5z FLOAT, from1 INT, from2 INT, from3 INT, from4 INT, from5 INT, bulNum1 INT, bulNum2 INT, bulNum3 INT, bulNum4 INT, bulNum5 INT, hitC1 INT, hitC2 INT, hitC3 INT, hitC4 INT, hitC5 INT, hp1 INT, hp2 INT, hp3 INT, hp4 INT, hp5 INT,  turHp1 INT, turHp2 INT, turHp3 INT, turHp4 INT, turHp5 INT, botHp1 INT, botHp2 INT, botHp3 INT, botHp4 INT, botHp5 INT, moveDis1 FLOAT, moveDis2 FLOAT, moveDis3 FLOAT, moveDis4 FLOAT, moveDis5 FLOAT, moveTim1 FLOAT, moveTim2 FLOAT, moveTim3 FLOAT, moveTim4 FLOAT, moveTim5 FLOAT;");
            return;
        }
        catch (Exception e)
        {
            Console.Write("[DataMgr]Connect " + e.Message);
            return;
        }
        finally
        {
            sqlConn.Close();
        }
	}

    //判断是否为安全字符
    //public bool IsSafeStr(string str)
	//{
	//	return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
	//}


	//执行非查询操作
	public void Execute(string cmdStr)
	{
        sqlConn = new MySqlConnection(connStr);
        MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
        try
        {
            sqlConn.Open();
            cmd.ExecuteNonQuery();
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine("[DataMgr]Register " + e.Message);
            return;
        }
        finally
        {
            sqlConn.Close();
        }
	}
	
    //获取Game_id
    // 只有房主可以调用这个
    // 游戏开始时调用一次
	public void GetGameId()
	{
		string str = "select gameId from gameId order by gameid DESC;";
		MySqlCommand cmd = new MySqlCommand (str, sqlConn); 
		try
		{
			MySqlDataReader dataReader = cmd.ExecuteReader(); 
			if (!dataReader.HasRows) {
				dataReader.Close();
                gameId = 1;
				string Str1="insert into gameId set gameId=1;";
				Execute(Str1);
				return;
			}
			dataReader.Read();
			int temp=1+dataReader.GetInt32(0);
			dataReader.Close();
			gameId=temp;
            // 这里写入map和wind参数
            string Str2 = string.Format("insert into gameId set gameId='{0}';", temp);
            Execute(Str2);
            return;
		}
		catch (MySqlException e) {
			Debug.Log ("[DataMgr]query " + e.Message);
			return;
		}
	}

    // operation 除掉gameid/gametime/player 剩下项如果全是0就不存，0.02秒检查一次
    public void StoreOperation(ZF.TankOrder _order)
    {
        string cmdstr = string.Format("insert into operation set gameId='{0}', gameTime='{1}', player='{2}', flag='{3}', move='{4}', " +
            "steer='{5}', derX='{6}', derY='{7}', derZ='{8}', shoot='{9}';", gameId, (float)PhotonNetwork.time, PhotonNetwork.player.ID, (int)PhotonNetwork.player.AllProperties["flag"], _order.move,
            _order.steer, _order.direction.x, _order.direction.y, _order.direction.z, _order.shoot);
        try
        {
            Execute(cmdstr);
            return;
        }
        catch(MySqlException e)
        {
            Debug.Log("[DataMgr]execution" + e.Message);
            return;
        }
    }
    // 必存，0.06秒检查一次

	public void StoreStatus(TankInfo _tankInfo)
	{
        string cmdstr = string.Format("insert into status set gameId='{0}', gameTime='{1}', player='{2}', flag='{3}', posX='{4}', " +
            "posY='{5}', posZ='{6}', rotX='{7}', rotY='{8}', rotZ='{9}', " +
            "tRotX='{10}', tRotY='{11}', tRotZ='{12}', vX='{13}', vY='{14}', vZ='{15}', hp='{16}';", gameId, (float)PhotonNetwork.time, PhotonNetwork.player.ID, (int)PhotonNetwork.player.AllProperties["flag"], _tankInfo.Position.x, _tankInfo.Position.y, _tankInfo.Position.z, _tankInfo.EularAngles.x, _tankInfo.EularAngles.y,
             _tankInfo.EularAngles.z, _tankInfo.TurretEularAngles.x, _tankInfo.TurretEularAngles.y, _tankInfo.TurretEularAngles.z, _tankInfo.Speed.x, _tankInfo.Speed.y, _tankInfo.Speed.z, _tankInfo.Health);
		try
		{
			Execute(cmdstr);
            return;
        }
		catch(MySqlException e)
		{
			Debug.Log("[DataMgr]execution" + e.Message);
			return;
		}
	}
    // 储存延时性范围武器（干扰弹...
    // 仅当发生事件时检查调用
    public void StoreSignal(Vector2 _position, float _r, float _deltaTime)
    {
        // gameId INT(10), gameTime float, player int, flag int, x float, y float, z float, r float, deltaTime float
        string cmdstr = string.Format("insert into status set gameId='{0}', gameTime='{1}', player='{2}', flag='{3}', x='{4}', " +
            "y='{5}', r='{6}', deltaTime='{7}';", gameId, (float)PhotonNetwork.time, PhotonNetwork.player.ID, (int)PhotonNetwork.player.AllProperties["flag"], _position.x, _position.y, _r, _deltaTime);
        try
        {
            Execute(cmdstr);
            return;
        }
        catch (MySqlException e)
        {
            Debug.Log("[DataMgr]execution" + e.Message);
            return;
        }
    }
    // 储存感知信息
    // 感知信息更新时同步储存
    public void StorePerceptor(PerceptInfo info)
    {
        string cmdstr = string.Format("INSERT INTO perception SET gameId='{0}', gameTime='{1}', player='{2}', flag='{3}', pos1x='{4}', pos1y='{5}', pos1z='{6}', pos2x='{7}', pos2y='{8}', pos2z='{9}', pos3x='{10}', pos3y='{11}', pos3z='{12}', pos4x='{13}', pos4y='{14}', pos4z='{15}', pos5x='{16}', pos5y='{17}', pos5z='{18}', from1='{19}', from2='{20}', from3='{21}', from4='{22}', from5='{23}', bulNum1='{24}', bulNum2='{25}', bulNum3='{26}', bulNum4='{27}', bulNum5='{28}', hitC1='{29}', hitC2='{30}', hitC3='{31}', hitC4='{32}', hitC5='{33}', hp1='{34}', hp2='{35}', hp3='{36}', hp4='{37}', hp5='{38}',  turHp1='{39}', turHp2='{40}', turHp3='{41}', turHp4='{42}', turHp5='{43}', botHp1='{44}', botHp2='{45}', botHp3='{46}', botHp4='{47}', botHp5='{48}', moveDis1='{49}', moveDis2='{50}', moveDis3='{51}', moveDis4='{51}', moveDis5='{52}', moveTim1='{53}', moveTim2='{54}', moveTim3='{55}', moveTim4='{56}', moveTim5='{57}';", gameId, (float)PhotonNetwork.time, PhotonNetwork.player.ID, (int)PhotonNetwork.player.AllProperties["flag"], info.positionPredict[0].x, info.positionPredict[0].y, info.positionPredict[0].z, info.positionPredict[1].x, info.positionPredict[1].y, info.positionPredict[1].z, info.positionPredict[2].x, info.positionPredict[2].y, info.positionPredict[2].z, info.positionPredict[3].x, info.positionPredict[3].y, info.positionPredict[3].z, info.positionPredict[4].x, info.positionPredict[4].y, info.positionPredict[4].z, info.attacktank[0], info.attacktank[1], info.attacktank[2], info.attacktank[3], info.attacktank[4], info.bulletNum[0], info.bulletNum[1], info.bulletNum[2], info.bulletNum[3], info.bulletNum[4], info.hitCount[0], info.hitCount[0], info.hitCount[0], info.hitCount[0], info.health[1], info.health[2], info.health[3], info.health[4], info.turHeal[0], info.turHeal[1], info.turHeal[2], info.turHeal[3], info.turHeal[4], info.botHeal[0], info.botHeal[1], info.botHeal[2], info.botHeal[3], info.botHeal[4], info.moveDis[0], info.moveDis[1], info.moveDis[2], info.moveDis[3], info.moveDis[4], info.moveTim[0], info.moveTim[1], info.moveTim[2], info.moveTim[3], info.moveTim[4]);
        try
        {
            Execute(cmdstr);
        }
        catch (MySqlException e)
        {
            Debug.Log("[DataMgr]execution" + e.Message);
            return;
        }
    }

    public void GetStatus(int _gameId, int _player, float _time, int _num)
    {
        TankInfo info = null;
        sqlConn = new MySqlConnection();
        MySqlCommand cmd = new MySqlCommand();
        cmd.CommandText = string.Format("SELECT * FROM status WHERE gameId='{0}' AND player='{1}' AND gameTime>'{2}'", _gameId, _player, _time);
        int i = 0;
        Vector3 temp = new Vector3();
        try
        {
            sqlConn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read() && i < _num)
            {
                temp.x = reader.GetFloat(reader.GetOrdinal("posX"));
                temp.y = reader.GetFloat(reader.GetOrdinal("posY"));
                temp.z = reader.GetFloat(reader.GetOrdinal("posZ"));
                info.Position = temp;
                temp.x = reader.GetFloat(reader.GetOrdinal("rotX"));
                temp.y = reader.GetFloat(reader.GetOrdinal("rotY"));
                temp.z = reader.GetFloat(reader.GetOrdinal("rotZ"));
                info.EularAngles = temp;
                temp.x = reader.GetFloat(reader.GetOrdinal("tRotX"));
                temp.y = reader.GetFloat(reader.GetOrdinal("tRotY"));
                temp.z = reader.GetFloat(reader.GetOrdinal("tRotZ"));
                info.TurretEularAngles = temp;
                temp.x = reader.GetFloat(reader.GetOrdinal("vX"));
                temp.y = reader.GetFloat(reader.GetOrdinal("vY"));
                temp.z = reader.GetFloat(reader.GetOrdinal("vZ"));
                info.Speed = temp;
                info.Health = reader.GetInt16(reader.GetOrdinal("hp"));
            }
        }
        catch (MySqlException e)
        {
            Debug.Log("[DataMgr]execution" + e.Message);
            return;
        }
        finally
        {
            sqlConn.Close();
        }
    }
}