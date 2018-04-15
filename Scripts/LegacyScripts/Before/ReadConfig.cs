using System;
using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;


namespace ConfigManager
{
    
    public class XMLConfigReader
    {
        Dictionary<string, string> TermDataDic = new Dictionary<string, string>();
        public void GetXMLInformation(string xmlFilePath, string ProgName, int choice)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Normalize();
                doc.Load(xmlFilePath);    //加载Xml文件  
                XmlElement rootElem = doc.DocumentElement;   //获取根节点  
                XmlNodeList TankNodes = rootElem.GetElementsByTagName("Tank"); //获取Tank子节点集合
                foreach (XmlNode node in TankNodes)
                {
                    int TankID = int.Parse(((XmlElement)node).GetAttribute("ID"));
                    //获取ID属性值  
                    if (TankID == choice)
                    {
                        XmlNodeList ConfigNodes = ((XmlElement)node).GetElementsByTagName("config");
                        foreach (XmlNode program in ConfigNodes)
                        {
                            string ConfigName = ((XmlElement)program).GetAttribute("name");
                            //获取name属性值 

                            if (ConfigName == ProgName)
                            {
                                Debug.LogWarning("set TankID = " + TankID +"  Config  "+ ConfigName);
                                foreach (XmlNode attr in program)
                                {
                                    //获取该节点的名字
                                    string name = attr.Name;
                                    //获取该节点的值（即：InnerText）
                                    string innerText = attr.InnerText;
                                    TermDataDic.Add(name, innerText);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Debug.LogWarning(ex);
            }
            Debug.Log("success");
        }

 /*      public void SetBullet(string xmlFilePath, string ProgName, Bullet bullet, int choice)
        {
            GetXMLInformation(xmlFilePath, ProgName,choice);
            bullet.speed = float.Parse(TermDataDic["speed"]);
            //bullet.explosion.name = TermDataDic["explosion.name"];
            bullet.maxExistTime = float.Parse(TermDataDic["maxExistTime"]);           
        }*/
        public void SetTankHealth(string xmlFilePath, string ProgName, TankHealth tankhealth,int choice)
        {
            GetXMLInformation(xmlFilePath, ProgName, choice);
            tankhealth.health = int.Parse(TermDataDic["health"]);
            tankhealth.protection = float.Parse(TermDataDic["protection"]);
            tankhealth.turret.health = int.Parse(TermDataDic["turret.health"]);
            tankhealth.turret.protection = float.Parse(TermDataDic["turret.protection"]);
            tankhealth.bottom.health = int.Parse(TermDataDic["bottom.health"]);
            tankhealth.bottom.protection = float.Parse(TermDataDic["bottom.protection"]);
        }
        public void SetTankControl(string xmlFilePath, string ProgName,TankControl tankcontrol, int choice)
        {
            GetXMLInformation(xmlFilePath, ProgName, choice);
            tankcontrol.shootInterval = float.Parse(TermDataDic["shootInterval"]);
            tankcontrol.turretYSpeed = float.Parse(TermDataDic["turretYSpeed"]);
            tankcontrol.gunXSpeed = float.Parse(TermDataDic["gunXSpeed"]);
            tankcontrol.gunXMax = float.Parse(TermDataDic["gunXMax"]);
            tankcontrol.gunXMin = float.Parse(TermDataDic["gunXMin"]);
            /*tankcontrol.maxRpm = int.Parse(TermDataDic["maxRpm"]);
            tankcontrol.motorTorque = float.Parse(TermDataDic["motorTorque"]);
            tankcontrol.brakeTorque = float.Parse(TermDataDic["brakeTorque"]);
            tankcontrol.steeringSpeed = float.Parse(TermDataDic["steeringSpeed"]);
            tankcontrol.maxSteering = float.Parse(TermDataDic["maxSteering"]);
            tankcontrol.steeringBack = float.Parse(TermDataDic["steeringBack"]);*/
        }
        public void SetTankWeapons(string xmlFilePath, string ProgName, TankWeapon tankweapons, int choice)
        {
            GetXMLInformation(xmlFilePath, ProgName, choice);
            tankweapons.Weapons[0].damage = int.Parse(TermDataDic["damage"]);
            tankweapons.Weapons[0].number = int.Parse(TermDataDic["number"]);
        }
    }
}
