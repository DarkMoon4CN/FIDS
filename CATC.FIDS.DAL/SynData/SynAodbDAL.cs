using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data;
using System.Text;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Data.SqlClient;
using MH.Common.Data;
using CATC.FIDS.Models;

namespace CATC.FIDS.DAL.SynData
{
    public class SynAodbDAL
    {
        /// <summary>
        /// Commom类中提供，引用MH.Common
        /// </summary>
        static AdoHelper aodb = MyDB.GetDBHelperByConnectionName("AODB_ConStr");
        static AdoHelper db = MyDB.GetDBHelperByConnectionName("FIDS_ConStr");

        public static bool CertificateCallback(Object sender, X509Certificate certificate, X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        #region 读取AODB基础信息
        /// <summary>
        /// 获取基础信息—航班
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAircraft()
        {
            string strSql = string.Format("SELECT AC_REG_NO,AC_TYPE_IATA,AIRLINE_IATA,SUBAIRLINE_ID,FLG_DELETED,EXT_CODE FROM saircraft");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—航空公司
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAirline()
        {
            string strSql = string.Format("SELECT Airline_IATA,Airline_ICAO,Short_Name,Host_AirPort_IATA,DORI,NAME_ENGLISH,NAME_CHINESE,ALLIANCE_CODE FROM sairline ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—航班类型
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAircraftType()  
        {
            string strSql = string.Format("select aircraft_type_iata,ac_type_icao,type_chinese,type_english from saircrafttype ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—机场
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAirport() 
        {
            string strSql = string.Format("SELECT airport_iata,airport_icao,airport_short_name,city_iata,dori,name_chinese,name_english,flg_deleted,region_code FROM sairport ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAlliance()
        {
            string strSql = string.Format("SELECT alliance_code,name_chinese,name_english,remark,flg_deleted FROM salliance ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—城市
        /// </summary>
        /// <returns></returns>
        public static DataSet GetCity()   
        {
            string strSql = string.Format("SELECT city_iata,country_iata,city_icao,name_chinese,name_english,short_chinese,province_id,flg_deleted FROM scity ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—国家
        /// </summary>
        /// <returns></returns>
        public static DataSet GetCountry()
        {
            string strSql = string.Format("SELECT country_iata,country_icao,name_chinese,name_english,flg_deleted FROM scountry ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—延时代码
        /// </summary>
        /// <returns></returns>
        public static DataSet GetDelayCode()   
        {
            string strSql = string.Format("SELECT delay_code, type, code_chinese, code_english, description, flg_deleted FROM sdelaycode");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—省份
        /// </summary>
        /// <returns></returns>
        public static DataSet GetProvince()  
        {
            string strSql = string.Format("SELECT province_id,province_cn,province_en,province_short_name,dori FROM sprovince");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—任务代码
        /// </summary>
        /// <returns></returns>
        public static DataSet GetTaskCode()
        {
            string strSql = string.Format("SELECT task_code, task_chinese, task_english, description, flg_deleted FROM staskcode");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取基础信息—
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAirlineSub()
        {
            string strSql = string.Format("SELECT id, parent_airline, subairline_name FROM ssubairline");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }
        /// <summary>
        /// 获取基础信息--登机口
        /// </summary>
        /// <returns></returns>
        public static DataSet GetGate()
        {
            string strSql = string.Format("SELECT facility_id,display_symbol,terminal_no,dori,description,status,status_timestamp,status_remark,recorder_id FROM fgate");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }
        /// <summary>
        /// 行李转盘
        /// </summary>
        /// <returns></returns>
        public static DataSet GetBagage()
        {
            string strSql = string.Format("SELECT facility_id,display_symbol,terminal_no,dori,description,status,status_timestamp,status_remark,recorder_id FROM FCAROUSEL_BAGGAGE");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }
        /// <summary>
        /// 值机柜台
        /// </summary>
        /// <returns></returns>
        public static DataSet GetCheckin()
        {
            string strSql = string.Format("SELECT facility_id,display_symbol,terminal_no,dori,description,status,status_timestamp,status_remark,recorder_id FROM FCHECKIN_COUNTER");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }
        #endregion

        #region 更新FIDS基础信息
        /// <summary>
        /// 航班信息
        /// </summary>
        /// <param name="airc_model"></param>
        public static int UpdateAircraft(F_Aircraft airc_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Aircraft WHERE AC_REG_NO='" + airc_model.AC_REG_NO + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count =Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count==1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Aircraft SET AC_TYPE_IATA='{0}',AIRLINE_IATA='{1}',SUBAIRLINE_ID='{2}',FLG_DELETED='{3}',EXT_CODE='{4}' WHERE AC_REG_NO='{5}'", airc_model.AC_TYPE_IATA, airc_model.AIRLINE_IATA, airc_model.SUBAIRLINE_ID, airc_model.FLG_DELETED, airc_model.EXT_CODE, airc_model.AC_REG_NO);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Aircraft (AC_REG_NO,AC_TYPE_IATA,AIRLINE_IATA,SUBAIRLINE_ID,FLG_DELETED,EXT_CODE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", airc_model.AC_REG_NO, airc_model.AC_TYPE_IATA, airc_model.AIRLINE_IATA, airc_model.SUBAIRLINE_ID, airc_model.FLG_DELETED, airc_model.EXT_CODE);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 航班类型
        /// </summary>
        /// <param name="airtype_model"></param>
        public static int UpdateAircraftType(F_AircraftType airtype_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_AircraftType WHERE iataCode='" + airtype_model.iataCode + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_AircraftType SET icaoCode='{0}',name_english='{1}',name_chinese='{2}' WHERE iataCode='{3}'", airtype_model.icaoCode, airtype_model.name_english, airtype_model.name_chinese, airtype_model.iataCode);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_AircraftType(iataCode,icaoCode,name_english,name_chinese)VALUES ('{0}','{1}','{2}','{3}')", airtype_model.iataCode, airtype_model.icaoCode, airtype_model.name_english, airtype_model.name_chinese);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }

        /// <summary>
        /// 航空公司
        /// </summary>
        /// <param name="airline_model"></param>
        public static int UpdateAirline(F_Airline airline_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Airline WHERE Airline_IATA='" + airline_model.Airline_IATA + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Airline SET Airline_ICAO='{0}',Short_Name='{1}',Host_AirPort_IATA='{2}',DORI='{3}',NAME_ENGLISH='{4}',NAME_CHINESE='{5}',ALLIANCE_CODE='{6}' WHERE Airline_IATA='{7}'", airline_model.Airline_ICAO, airline_model.Short_Name, airline_model.Host_AirPort_IATA, airline_model.DORI, airline_model.NAME_ENGLISH, airline_model.NAME_CHINESE, airline_model.ALLIANCE_CODE, airline_model.Airline_IATA);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Airline(Airline_IATA,Airline_ICAO,Short_Name,Host_AirPort_IATA,DORI,NAME_ENGLISH,NAME_CHINESE,ALLIANCE_CODE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", airline_model.Airline_IATA, airline_model.Airline_ICAO, airline_model.Short_Name, airline_model.Host_AirPort_IATA, airline_model.DORI, airline_model.NAME_ENGLISH, airline_model.NAME_CHINESE, airline_model.ALLIANCE_CODE);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 机场
        /// </summary>
        /// <param name="airp_model"></param>
        public static int UpdateAirport(F_Airport airp_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Airport WHERE AIRPORT_IATA='" + airp_model.AIRPORT_IATA + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Airport SET AIRPORT_ICAO='{0}',SHORT_NAME='{1}',CITY_IATA='{2}',DORI='{3}',NAME_CHINESE='{4}',NAME_ENGLISH='{5}',FLG_DELETED='{6}',REGION_CODE='{7}' WHERE AIRPORT_IATA='{8}'", airp_model.AIRPORT_ICAO, airp_model.SHORT_NAME, airp_model.CITY_IATA, airp_model.DORI, airp_model.NAME_CHINESE, airp_model.NAME_ENGLISH, airp_model.FLG_DELETED, airp_model.REGION_CODE, airp_model.AIRPORT_IATA);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Airport(AIRPORT_IATA,AIRPORT_ICAO,SHORT_NAME,CITY_IATA,DORI,NAME_CHINESE,NAME_ENGLISH,FLG_DELETED,REGION_CODE)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", airp_model.AIRPORT_IATA, airp_model.AIRPORT_ICAO, airp_model.SHORT_NAME, airp_model.CITY_IATA, airp_model.DORI, airp_model.NAME_CHINESE, airp_model.NAME_ENGLISH, airp_model.FLG_DELETED, airp_model.REGION_CODE);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="all_model"></param>
        public static int UpdateAlliance(F_Alliance all_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Alliance WHERE ALLIANCE_NAME='" + all_model.ALLIANCE_NAME + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Alliance SET NAME_CHINESE='{0}',NAME_ENGLISH='{1}',REMARK='{2}',FLG_DELETED='{3}' WHERE ALLIANCE_NAME='{4}'", all_model.NAME_CHINESE, all_model.NAME_ENGLISH, all_model.REMARK, all_model.FLG_DELETED, all_model.ALLIANCE_NAME);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Alliance(ALLIANCE_NAME,NAME_CHINESE,NAME_ENGLISH,REMARK,FLG_DELETED)VALUES ('{0}','{1}','{2}','{3}','{4}')", all_model.ALLIANCE_NAME, all_model.NAME_CHINESE, all_model.NAME_ENGLISH, all_model.REMARK, all_model.FLG_DELETED);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 城市
        /// </summary>
        /// <param name="city_model"></param>
        public static int UpdateCity(F_City city_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_City WHERE City_IATA='" + city_model.City_IATA + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_City SET Country_IATA='{0}',City_ICAO='{1}',Name_Chinese='{2}',Name_English='{3}',Short_Chinese='{4}',Province_IS='{5}',FLG_Deleted='{6}' WHERE City_IATA='{7}'", city_model.Country_IATA, city_model.City_ICAO, city_model.Name_Chinese, city_model.Name_English, city_model.Short_Chinese, city_model.Province_IS, city_model.FLG_Deleted, city_model.City_IATA);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_City(City_IATA,Country_IATA,City_ICAO,Name_Chinese,Name_English,Short_Chinese,Province_IS,FLG_Deleted)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", city_model.City_IATA, city_model.Country_IATA, city_model.City_ICAO, city_model.Name_Chinese, city_model.Name_English, city_model.Short_Chinese, city_model.Province_IS, city_model.FLG_Deleted);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 国家
        /// </summary>
        /// <param name="coun_model"></param>
        public static int UpdateCountry(F_Country coun_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Country WHERE Country_IATA='" + coun_model.Country_IATA + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Country SET Country_ICAO='{0}',Name_Chinese='{1}',Name_English='{2}',FLG_Deleted='{3}' WHERE Country_IATA='{4}'", coun_model.Country_ICAO, coun_model.Name_Chinese, coun_model.Name_English, coun_model.FLG_Deleted, coun_model.Country_IATA);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Country(Country_IATA,Country_ICAO,Name_Chinese,Name_English,FLG_Deleted)VALUES ('{0}','{1}','{2}','{3}','{4}')", coun_model.Country_IATA, coun_model.Country_ICAO, coun_model.Name_Chinese, coun_model.Name_English, coun_model.FLG_Deleted);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }

        /// <summary>
        /// 延误代码
        /// </summary>
        /// <param name="del_model"></param>
        public static int UpdateDelayCode(F_DelayCode del_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_DelayCode WHERE Delay_Code='" + del_model.Delay_Code + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_DelayCode SET Type='{0}',Code_Chinese='{1}',Code_English='{2}',Description='{3}',FLG_Deleted='{4}' WHERE Delay_Code='{5}'", del_model.Type, del_model.Code_Chinese, del_model.Code_English, del_model.Description, del_model.FLG_Deleted, del_model.Delay_Code);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_DelayCode(Delay_Code,Type,Code_Chinese,Code_English,Description,FLG_Deleted)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')", del_model.Delay_Code, del_model.Type, del_model.Code_Chinese, del_model.Code_English, del_model.Description, del_model.FLG_Deleted);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 省份
        /// </summary>
        /// <param name="del_model"></param>
        public static int UpdateProvince(F_Province prov_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Province WHERE Province_ID='" + prov_model.Province_ID + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Province SET Name_Chinese='{0}',Name_English='{1}',Short_Name='{2}',DORI='{3}' WHERE Province_ID='{4}'", prov_model.Name_Chinese, prov_model.Name_English, prov_model.Short_Name, prov_model.DORI, prov_model.Province_ID);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Province(Province_ID,Name_Chinese,Name_English,Short_Name,DORI)VALUES ('{0}','{1}','{2}','{3}','{4}')", prov_model.Province_ID, prov_model.Name_Chinese, prov_model.Name_English, prov_model.Short_Name, prov_model.DORI);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 任务代号
        /// </summary>
        /// <param name="prov_model"></param>
        public static int UpdateTaskCode(F_TaskCode task_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_TaskCode WHERE Task_Code='" + task_model.Task_Code + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_TaskCode SET Name_Chinese='{0}',Name_English='{1}',Description='{2}',FLG_Deleted='{3}' WHERE Task_Code='{4}'", task_model.Name_Chinese, task_model.Name_English, task_model.Description, task_model.FLG_Deleted, task_model.Task_Code);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_TaskCode(Task_Code,Name_Chinese,Name_English,Description,FLG_Deleted)VALUES ('{0}','{1}','{2}','{3}','{4}')", task_model.Task_Code, task_model.Name_Chinese, task_model.Name_English, task_model.Description, task_model.FLG_Deleted);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        /// <summary>
        /// 航空子公司
        /// </summary>
        /// <param name="airsub_model"></param>
        public static int UpdateAirlineSub(F_Airline_Sub airsub_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.F_Airline_Sub WHERE ID='" + airsub_model.ID + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.F_Airline_Sub SET Parent_Airline='{0}',Subairline_Name='{1}' WHERE ID='{2}'", airsub_model.Parent_Airline, airsub_model.Subairline_Name, airsub_model.ID);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"insert into dbo.F_Airline_Sub(ID,Parent_Airline,Subairline_Name)VALUES ('{0}','{1}','{2}')", airsub_model.ID, airsub_model.Parent_Airline, airsub_model.Subairline_Name);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }

        /// <summary>
        /// 登机口
        /// </summary>
        /// <param name="airsub_model"></param>
        public static int UpdateFgate(R_Facility gate_model)
        {
            string strSql = "";
            string sql = "SELECT  COUNT(1) as num FROM dbo.R_Facility WHERE Aodb_Facility_ID='" + gate_model.Aodb_Facility_ID + "'";
            DataSet ds = db.ExecuteDataSet(sql);
            int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
            if (count == 1)
            {
                strSql = string.Format(@"UPDATE dbo.R_Facility SET Display_Symbol='{0}',Terminal_NO='{1}',DORI='{2}',Description='{3}',Status='{4}',Status_Timestamp='{5}',Status_Remark='{6}',Recorder_ID='{7}' WHERE Aodb_Facility_ID='{8}'", gate_model.Display_Symbol, gate_model.Terminal_NO, gate_model.DORI, gate_model.Description, gate_model.Status, gate_model.Status_Timestamp, gate_model.Status_Remark, gate_model.Recorder_ID, gate_model.Aodb_Facility_ID);
                db.ExecuteNonQuery(strSql);
            }
            else
            {
                strSql = string.Format(@"INSERT INTO dbo.R_Facility(Aodb_Facility_ID,Display_Symbol,Terminal_NO,DORI,Description,Status,Status_Timestamp,Status_Remark,Recorder_ID)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", gate_model.Aodb_Facility_ID, gate_model.Display_Symbol, gate_model.Terminal_NO, gate_model.DORI, gate_model.Description, gate_model.Status, gate_model.Status_Timestamp, gate_model.Status_Remark, gate_model.Recorder_ID);
                db.ExecuteNonQuery(strSql);
            }
            return count;
        }
        #endregion

        /// <summary>
        /// 初始化写入基础信息
        /// </summary>
        /// <param name="DataSet"></param>
        public static int SaveBasicsData(DataSet ds_airc,DataSet ds_airl,DataSet ds_airtype, DataSet ds_airport, DataSet ds_alliance,DataSet ds_city,DataSet ds_country,DataSet ds_delcode,DataSet ds_prov,DataSet ds_task,DataSet ds_airlsub,DataSet ds_gate,DataSet ds_bagage, DataSet ds_checkin)
        {
            int result = -1;
            StringBuilder strB = new StringBuilder();
            //string strSql = string.Empty;
            try
            {
                #region 写入航班表
                if (ds_airc.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_airc.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Aircraft (AC_REG_NO,AC_TYPE_IATA,AIRLINE_IATA,SUBAIRLINE_ID,FLG_DELETED,EXT_CODE) VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", ds_airc.Tables[0].Rows[i]["AC_REG_NO"].ToString(), ds_airc.Tables[0].Rows[i]["AC_TYPE_IATA"].ToString(), ds_airc.Tables[0].Rows[i]["AIRLINE_IATA"].ToString(), ds_airc.Tables[0].Rows[i]["SUBAIRLINE_ID"].ToString(), ds_airc.Tables[0].Rows[i]["FLG_DELETED"].ToString(), ds_airc.Tables[0].Rows[i]["EXT_CODE"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_Aircraft";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入航空公司表
                if (ds_airl.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_airl.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Airline (Airline_IATA,Airline_ICAO,Short_Name,Host_AirPort_IATA,DORI,NAME_ENGLISH,NAME_CHINESE,ALLIANCE_CODE) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", ds_airl.Tables[0].Rows[i]["Airline_IATA"].ToString(), ds_airl.Tables[0].Rows[i]["Airline_ICAO"].ToString(), ds_airl.Tables[0].Rows[i]["Short_Name"].ToString(), ds_airl.Tables[0].Rows[i]["Host_AirPort_IATA"].ToString(), ds_airl.Tables[0].Rows[i]["DORI"].ToString(), ds_airl.Tables[0].Rows[i]["NAME_ENGLISH"].ToString().Replace("'", ""), ds_airl.Tables[0].Rows[i]["NAME_CHINESE"].ToString(), ds_airl.Tables[0].Rows[i]["ALLIANCE_CODE"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_Airline";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入航班类型表
                //写入航班类型表
                if (ds_airtype.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_airtype.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_AircraftType(iataCode,icaoCode,name_english,name_chinese)VALUES ('{0}','{1}','{2}','{3}')", ds_airtype.Tables[0].Rows[i]["aircraft_type_iata"].ToString(), ds_airtype.Tables[0].Rows[i]["ac_type_icao"].ToString(), ds_airtype.Tables[0].Rows[i]["type_english"].ToString().Replace("'", ""), ds_airtype.Tables[0].Rows[i]["type_chinese"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_AircraftType";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入机场信息表
                //写入机场信息表
                if (ds_airport.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_airport.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Airport(AIRPORT_IATA,AIRPORT_ICAO,SHORT_NAME,CITY_IATA,DORI,NAME_CHINESE,NAME_ENGLISH,FLG_DELETED,REGION_CODE)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", ds_airport.Tables[0].Rows[i]["airport_iata"].ToString(), ds_airport.Tables[0].Rows[i]["airport_icao"].ToString(), ds_airport.Tables[0].Rows[i]["airport_short_name"].ToString(), ds_airport.Tables[0].Rows[i]["city_iata"].ToString(), ds_airport.Tables[0].Rows[i]["dori"].ToString(), ds_airport.Tables[0].Rows[i]["name_chinese"].ToString(), ds_airport.Tables[0].Rows[i]["name_english"].ToString().Replace("'", ""), ds_airport.Tables[0].Rows[i]["flg_deleted"].ToString(), ds_airport.Tables[0].Rows[i]["region_code"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_Airport";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入alliance
                //写入alliance
                if (ds_alliance.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_alliance.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Alliance(ALLIANCE_NAME,NAME_CHINESE,NAME_ENGLISH,REMARK,FLG_DELETED)VALUES ('{0}','{1}','{2}','{3}','{4}')", ds_alliance.Tables[0].Rows[i]["alliance_code"].ToString(), ds_alliance.Tables[0].Rows[i]["name_chinese"].ToString(), ds_alliance.Tables[0].Rows[i]["name_english"].ToString().Replace("'", ""), ds_alliance.Tables[0].Rows[i]["remark"].ToString(), ds_alliance.Tables[0].Rows[i]["flg_deleted"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_Alliance";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入城市信息
                //写入城市信息
                if (ds_city.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_city.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_City(City_IATA,Country_IATA,City_ICAO,Name_Chinese,Name_English,Short_Chinese,Province_IS,FLG_Deleted)VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", ds_city.Tables[0].Rows[i]["city_iata"].ToString(), ds_city.Tables[0].Rows[i]["country_iata"].ToString(), ds_city.Tables[0].Rows[i]["city_icao"].ToString(), ds_city.Tables[0].Rows[i]["name_chinese"].ToString(), ds_city.Tables[0].Rows[i]["name_english"].ToString().Replace("'", ""), ds_city.Tables[0].Rows[i]["short_chinese"].ToString(), ds_city.Tables[0].Rows[i]["province_id"].ToString(), ds_city.Tables[0].Rows[i]["flg_deleted"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_City";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入国家信息
                //写入国家信息
                if (ds_country.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_country.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Country(Country_IATA,Country_ICAO,Name_Chinese,Name_English,FLG_Deleted)VALUES ('{0}','{1}','{2}','{3}','{4}')", ds_country.Tables[0].Rows[i]["country_iata"].ToString(), ds_country.Tables[0].Rows[i]["country_icao"].ToString(), ds_country.Tables[0].Rows[i]["name_chinese"].ToString(), ds_country.Tables[0].Rows[i]["name_english"].ToString().Replace("'", ""), ds_country.Tables[0].Rows[i]["flg_deleted"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_Country";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入延误代码信息
                //写入延误代码信息
                if (ds_delcode.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_delcode.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_DelayCode(Delay_Code,Type,Code_Chinese,Code_English,Description,FLG_Deleted)VALUES('{0}','{1}','{2}','{3}','{4}','{5}')", ds_delcode.Tables[0].Rows[i]["delay_code"].ToString(), ds_delcode.Tables[0].Rows[i]["type"].ToString(), ds_delcode.Tables[0].Rows[i]["code_chinese"].ToString(), ds_delcode.Tables[0].Rows[i]["code_english"].ToString().Replace("'", ""), ds_delcode.Tables[0].Rows[i]["description"].ToString(), ds_delcode.Tables[0].Rows[i]["flg_deleted"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_DelayCode";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入省份信息
                //写入省份信息
                if (ds_prov.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_prov.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Province(Name_Chinese,Name_English,Short_Name ,DORI)VALUES('{0}','{1}','{2}','{3}')", ds_prov.Tables[0].Rows[i]["province_cn"].ToString(), ds_prov.Tables[0].Rows[i]["province_en"].ToString(), ds_prov.Tables[0].Rows[i]["province_short_name"].ToString(), ds_prov.Tables[0].Rows[i]["dori"].ToString());
                    }
                    string sql = "delete from dbo.F_Province";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入任务代码
                //写入任务代码
                if (ds_task.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_task.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_TaskCode(Task_Code,Name_Chinese,Name_English,Description,FLG_Deleted)VALUES('{0}','{1}','{2}','{3}','{4}')", ds_task.Tables[0].Rows[i]["task_code"].ToString(), ds_task.Tables[0].Rows[i]["task_chinese"].ToString(), ds_task.Tables[0].Rows[i]["task_english"].ToString().Replace("'", ""), ds_task.Tables[0].Rows[i]["description"].ToString(), ds_task.Tables[0].Rows[i]["flg_deleted"].ToString());
                    }
                    string sql = "delete from dbo.F_TaskCode";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入航空子公司
                //写入航空子公司
                if (ds_airlsub.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    for (int i = 0; i < ds_airlsub.Tables[0].Rows.Count; i++)
                    {
                        strSql = strSql + string.Format(@"insert into dbo.F_Airline_Sub(ID,Parent_Airline,Subairline_Name)VALUES('{0}','{1}','{2}')", ds_airlsub.Tables[0].Rows[i]["id"].ToString(), ds_airlsub.Tables[0].Rows[i]["parent_airline"].ToString(), ds_airlsub.Tables[0].Rows[i]["subairline_name"].ToString());
                        //strB.Append(strSql);
                    }
                    string sql = "delete from dbo.F_Airline_Sub";
                    db.ExecuteNonQuery(sql);
                    result = db.ExecuteNonQuery(strSql.ToString());
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入登机口信息
                //写入登机口信息
                if (ds_gate.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    string sql = "SELECT COUNT(1) as num FROM dbo.R_Facility  where FacilityType=1";
                    DataSet ds = db.ExecuteDataSet(sql);
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
                    if (count <= 0)
                    {
                        for (int i = 0; i < ds_gate.Tables[0].Rows.Count; i++)
                        {
                            strSql = strSql + string.Format(@"insert into dbo.R_Facility(Aodb_Facility_ID,Display_Symbol,Terminal_NO,DORI,Description,Status,Status_Timestamp,Status_Remark,Recorder_ID,FacilityType)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", ds_gate.Tables[0].Rows[i]["facility_id"].ToString(), ds_gate.Tables[0].Rows[i]["display_symbol"].ToString(), ds_gate.Tables[0].Rows[i]["terminal_no"].ToString(), ds_gate.Tables[0].Rows[i]["dori"].ToString(), ds_gate.Tables[0].Rows[i]["description"].ToString(), ds_gate.Tables[0].Rows[i]["status"].ToString(), ds_gate.Tables[0].Rows[i]["status_timestamp"].ToString(), ds_gate.Tables[0].Rows[i]["status_remark"].ToString(), ds_gate.Tables[0].Rows[i]["recorder_id"].ToString(),'1');
                        }
                        result = db.ExecuteNonQuery(strSql);
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入行李转盘信息
                //写入行李转盘信息
                if (ds_bagage.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    string sql = "SELECT COUNT(1) as num FROM dbo.R_Facility where FacilityType=3";
                    DataSet ds = db.ExecuteDataSet(sql);
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
                    if (count <= 0)
                    {
                        for (int i = 0; i < ds_bagage.Tables[0].Rows.Count; i++)
                        {
                            strSql = strSql + string.Format(@"insert into dbo.R_Facility(Aodb_Facility_ID,Display_Symbol,Terminal_NO,DORI,Description,Status,Status_Timestamp,Status_Remark,Recorder_ID,FacilityType)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", ds_bagage.Tables[0].Rows[i]["facility_id"].ToString(), ds_bagage.Tables[0].Rows[i]["display_symbol"].ToString(), ds_bagage.Tables[0].Rows[i]["terminal_no"].ToString(), ds_bagage.Tables[0].Rows[i]["dori"].ToString(), ds_bagage.Tables[0].Rows[i]["description"].ToString(), ds_bagage.Tables[0].Rows[i]["status"].ToString(), ds_bagage.Tables[0].Rows[i]["status_timestamp"].ToString(), ds_bagage.Tables[0].Rows[i]["status_remark"].ToString(), ds_bagage.Tables[0].Rows[i]["recorder_id"].ToString(), '3');
                        }
                        result = db.ExecuteNonQuery(strSql);
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    return -1;
                }
                #endregion

                #region 写入值机柜台信息
                //写入值机柜台信息
                if (ds_checkin.Tables[0].Rows.Count > 0)
                {
                    string strSql = string.Empty;
                    string sql = "SELECT COUNT(1) as num FROM dbo.R_Facility where FacilityType=2";
                    DataSet ds = db.ExecuteDataSet(sql);
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
                    if (count <= 0)
                    {
                        for (int i = 0; i < ds_checkin.Tables[0].Rows.Count; i++)
                        {
                            strSql = strSql + string.Format(@"insert into dbo.R_Facility(Aodb_Facility_ID,Display_Symbol,Terminal_NO,DORI,Description,Status,Status_Timestamp,Status_Remark,Recorder_ID,FacilityType)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", ds_checkin.Tables[0].Rows[i]["facility_id"].ToString(), ds_checkin.Tables[0].Rows[i]["display_symbol"].ToString(), ds_checkin.Tables[0].Rows[i]["terminal_no"].ToString(), ds_checkin.Tables[0].Rows[i]["dori"].ToString(), ds_checkin.Tables[0].Rows[i]["description"].ToString(), ds_checkin.Tables[0].Rows[i]["status"].ToString(), ds_checkin.Tables[0].Rows[i]["status_timestamp"].ToString(), ds_checkin.Tables[0].Rows[i]["status_remark"].ToString(), ds_checkin.Tables[0].Rows[i]["recorder_id"].ToString(), '2');
                        }
                        result = db.ExecuteNonQuery(strSql);
                    }
                    else
                    {
                        result = 0;
                    }
                }
                else
                {
                    return -1;
                }
                #endregion

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }



        #region 事件处理相关
        /// <summary>
        /// 获取未处理的事件信息
        /// </summary>
        /// <returns></returns>
        public static DataSet GetEvent()
        {
            string str = "N";
            string strSql = string.Format("select id,associate_id,event_code,issue_timestamp from i_fids_event where is_processed='{0}'", str);
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 通过ID获取基础数据变更接口表
        /// </summary>
        /// <returns></returns>
        public static DataSet GetBasic_chg(string associate_id)
        {
            string strSql = string.Format("SELECT flg_idu,table_name,pk_name,pk_value,received_time FROM i_fids_basic_chg where id='{0}'", associate_id);
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 读取AODB基础表
        /// </summary>
        /// <param name="tb_val">表名</param>
        /// <param name="str_val">字段名</param>
        /// <param name="in_val">值</param>
        /// <returns></returns>
        public static DataSet GetBasicData(string tb_val, string str_val, string in_val)
        {
            string strSql = string.Format("SELECT * FROM "+ tb_val + " where "+ str_val + "='"+ in_val + "'");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }


        /// <summary>
        /// 通过ID获取航班资源(机位、登机口)分配数据数据表
        /// </summary>
        /// <returns></returns>
        public static DataSet GetFlight_res(string associate_id)
        {
            string strSql = string.Format("select id,operation_date,flight_no,aord,bay_sched_id,bay_sched_start,bay_sched_end,bay_estimate_id,bay_estimate_start,bay_estimate_end,bay_actual_id,bay_actual_start,bay_actual_end,gate_sched_id,gate_sched_start,gate_sched_end,gate_estimate_id,gate_estimate_start,gate_estimate_end,gate_actual_id,gate_actual_start,gate_actual_end from i_fids_flight_res where id='{0}'", associate_id);
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }
        /// <summary>
        /// 读取全部航班资源分配信息
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllFlight_res()
        {
            //取全部记录前，先删除FIDS中前一天的资源分配数据
            string sql = "delete from Flight_Resource_Allocation";
            db.ExecuteNonQuery(sql);
            
            string strSql = string.Format("select id,operation_date,flight_no,aord,bay_sched_id,bay_sched_start,bay_sched_end,bay_estimate_id,bay_estimate_start,bay_estimate_end,bay_actual_id,bay_actual_start,bay_actual_end,gate_sched_id,gate_sched_start,gate_sched_end,gate_estimate_id,gate_estimate_start,gate_estimate_end,gate_actual_id,gate_actual_start,gate_actual_end from i_fids_flight_res");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }


        /// <summary>
        /// 通过AODB资源ID获取FIDS资源ID
        /// </summary>
        /// <param name="sched_id"></param>
        /// <returns></returns>
        public static DataSet GetFacilityID(string sched_id)
        {
            string strSql = string.Format("select Facility_ID from R_Facility where Aodb_Facility_ID='{0}'", sched_id);
            DataSet ds = db.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 将AODB数据更新或写入 航班资源分配数据
        /// </summary>
        public static int InsertFlight_Res(Flight_Resource_Allocation flight_rmodel)
        {
            try
            {
                string strSql = "";
                string sql = "SELECT  COUNT(1) as num FROM dbo.Flight_Resource_Allocation WHERE ID='" + flight_rmodel.ID + "'";
                DataSet ds = db.ExecuteDataSet(sql);
                int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
                if (count == 1)
                {
                    strSql = string.Format(@"UPDATE dbo.Flight_Resource_Allocation SET OPERATION_DATE='{0}',FacilityType='{1}',FLIGHT_NO='{2}',AORD='{3}',SCHED_ID='{4}',SCHED_START='{5}',SCHED_END='{6}',ESTIMATE_ID='{7}',ESTIMATE_START='{8}',ESTIMATE_END='{9}',ACTUAL_ID='{10}',ACTUAL_START='{11}',ACTUAL_END='{12}' WHERE ID='{13}'", flight_rmodel.OPERATION_DATE, flight_rmodel.FacilityType, flight_rmodel.FLIGHT_NO, flight_rmodel.AORD, flight_rmodel.SCHED_ID, flight_rmodel.SCHED_START, flight_rmodel.SCHED_END, flight_rmodel.ESTIMATE_ID, flight_rmodel.ESTIMATE_START, flight_rmodel.ESTIMATE_END, flight_rmodel.ACTUAL_ID, flight_rmodel.ACTUAL_START, flight_rmodel.ACTUAL_END, flight_rmodel.ID);
                    db.ExecuteNonQuery(strSql);
                }
                else
                {
                    strSql = string.Format(@"INSERT INTO dbo.Flight_Resource_Allocation(ID,OPERATION_DATE,FacilityType,FLIGHT_NO,AORD,SCHED_ID,SCHED_START,SCHED_END,ESTIMATE_ID,ESTIMATE_START,ESTIMATE_END,ACTUAL_ID,ACTUAL_START,ACTUAL_END)VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", flight_rmodel.ID, flight_rmodel.OPERATION_DATE, flight_rmodel.FacilityType, flight_rmodel.FLIGHT_NO, flight_rmodel.AORD, flight_rmodel.SCHED_ID, flight_rmodel.SCHED_START, flight_rmodel.SCHED_END, flight_rmodel.ESTIMATE_ID, flight_rmodel.ESTIMATE_START, flight_rmodel.ESTIMATE_END, flight_rmodel.ACTUAL_ID, flight_rmodel.ACTUAL_START, flight_rmodel.ACTUAL_END);
                    db.ExecuteNonQuery(strSql);
                }
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }


        public static int InsertAllFlight_Res(Flight_Resource_Allocation flight_rmodel)
        {
            try
            {
                string sql = "delete from dbo.Flight_Resource_Allocation where FLIGHT_NO='" + flight_rmodel.FLIGHT_NO + "'";
                db.ExecuteNonQuery(sql);

                string strSql = string.Format(@"INSERT INTO dbo.Flight_Resource_Allocation(ID,OPERATION_DATE,FacilityType,FLIGHT_NO,AORD,SCHED_ID,SCHED_START,SCHED_END,ESTIMATE_ID,ESTIMATE_START,ESTIMATE_END,ACTUAL_ID,ACTUAL_START,ACTUAL_END)VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", flight_rmodel.ID, flight_rmodel.OPERATION_DATE, flight_rmodel.FacilityType, flight_rmodel.FLIGHT_NO, flight_rmodel.AORD, flight_rmodel.SCHED_ID, flight_rmodel.SCHED_START, flight_rmodel.SCHED_END, flight_rmodel.ESTIMATE_ID, flight_rmodel.ESTIMATE_START, flight_rmodel.ESTIMATE_END, flight_rmodel.ACTUAL_ID, flight_rmodel.ACTUAL_START, flight_rmodel.ACTUAL_END);
                return db.ExecuteNonQuery(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 通过ID获取航班动态数据接口表
        /// </summary>
        /// <returns></returns>
        public static DataSet GetFlight_Curday(string associate_id)
        {
            string strSql = string.Format("select id,flg_idu,operation_date,flight_no,aord,dori,task_code,terminal_no,airline_iata,aircraft_type_iata,ac_reg_no,service_class,flg_vip,origin_airport_iata,std,etd,atd,dest_airport_iata,sta,eta,ata,previous_flight,next_flight,abnormal_status,airport1,dori1,std1,airport2,dori2,sta2,std2,airport3,dori3,sta3,std3,airport7,sta7,code_share1,code_share2,code_share3,code_share4,cause_of_abnormal_en,cause_of_abnormal_cn,alt_airport from i_fids_curday_flight where id='{0}'", associate_id);
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        /// <summary>
        /// 获取全部航班动态数据（当日）
        /// </summary>
        /// <returns></returns>
        public static DataSet GetAllFlight_Curday()
        {
            string strSql = string.Format("select id,flg_idu,operation_date,flight_no,aord,dori,task_code,terminal_no,airline_iata,aircraft_type_iata,ac_reg_no,service_class,flg_vip,origin_airport_iata,std,etd,atd,dest_airport_iata,sta,eta,ata,previous_flight,next_flight,abnormal_status,airport1,dori1,std1,airport2,dori2,sta2,std2,airport3,dori3,sta3,std3,airport7,sta7,code_share1,code_share2,code_share3,code_share4,cause_of_abnormal_en,cause_of_abnormal_cn,alt_airport from i_fids_curday_flight ");
            DataSet ds = aodb.ExecuteDataSet(strSql);
            return ds;
        }

        
        /// <summary>
        /// 备份至航班数据历史库，成功后删除航班数据
        /// </summary>
        /// <returns></returns>
        public static int BackUpFlight_Dynamic()
        {
            string strSql = string.Format("INSERT INTO dbo.History_Flight_Dynamic SELECT ID,FLG_IDU,OPERATION_DATE,FLIGHT_NO,AORD,DORI,TASK_CODE,TERMINAL_NO,AIRLINE_IATA,AIRCRAFT_TYPE_IATA,AC_REG_NO,SERVICE_CLASS,FLG_VIP,ORIGIN_AIRPORT_IATA,STD,ETD,ATD,DEST_AIRPORT_IATA,STA,ETA,ATA,PREVIOUS_FLIGHT,NEXT_FLIGHT,ABNORMAL_STATUS,AIRPORT1,DORI1,STD1,AIRPORT2,DORI2,STA2,STD2,AIRPORT3,DORI3,STA3,STD3,AIRPORT4,DORI4,STA4,CODE_SHARE1,CODE_SHARE2,CODE_SHARE3,CODE_SHARE4,ADD_TYPE,Status_Code FROM dbo.Flight_Dynamic");
            int result = db.ExecuteNonQuery(strSql);
            if (result > 0)
            {
                string sql = "DELETE FROM dbo.Flight_Dynamic";
                db.ExecuteNonQuery(sql);
            }
            return result;
        }

        /// <summary>
        /// 将AODB数据写入航班数据表
        /// </summary>
        public static int InsertFlight_Curday(Flight_Dynamic flight_model)
        {
            try
            {
                string strSql = "";
                string sql = "SELECT  COUNT(1) as num FROM dbo.Flight_Dynamic WHERE FLIGHT_NO='" + flight_model.FLIGHT_NO + "'";
                DataSet ds = db.ExecuteDataSet(sql);
                int count = Convert.ToInt32(ds.Tables[0].Rows[0]["num"].ToString());
                if (count == 1)
                {
                    strSql = string.Format(@"UPDATE dbo.Flight_Dynamic SET  FLG_IDU='{0}',OPERATION_DATE='{1}',AORD='{2}',DORI='{3}',TASK_CODE='{4}',TERMINAL_NO='{5}',AIRLINE_IATA='{6}',AIRCRAFT_TYPE_IATA='{7}',AC_REG_NO='{8}',SERVICE_CLASS='{9}',FLG_VIP='{10}',ORIGIN_AIRPORT_IATA='{11}',STD='{12}',ETD={13},ATD='{14}',DEST_AIRPORT_IATA='{15}',STA='{16}',ETA='{17}',ATA='{18}',PREVIOUS_FLIGHT='{19}',NEXT_FLIGHT='{20}',ABNORMAL_STATUS='{21}',AIRPORT1='{22}',DORI1='{23}',STD1='{24}',AIRPORT2='{25}',DORI2='{26}',STA2='{27}',STD2='{28}',AIRPORT3='{29}',DORI3='{30}',STA3='{31}',STD3='{32}',AIRPORT4='{33}',DORI4='{34}',STA4='{35}',CODE_SHARE1='{36}',CODE_SHARE2='{37}',CODE_SHARE3='{38}',CODE_SHARE4='{39}',ADD_TYPE='{40}',Status_Code='{41}',Modify_Date='{42}' WHERE FLIGHT_NO='{43}'", flight_model.FLG_IDU, flight_model.OPERATION_DATE, flight_model.AORD, flight_model.DORI, flight_model.TASK_CODE, flight_model.TERMINAL_NO, flight_model.AIRLINE_IATA, flight_model.AIRCRAFT_TYPE_IATA, flight_model.AC_REG_NO, flight_model.SERVICE_CLASS, flight_model.FLG_VIP, flight_model.ORIGIN_AIRPORT_IATA, flight_model.STD, flight_model.ETD == null ? "NULL": "'"+flight_model.ETD+"'", flight_model.ATD == null ? "NULL" : "'" + flight_model.ATD + "'", flight_model.DEST_AIRPORT_IATA, flight_model.STA, flight_model.ETA == null ? "NULL" : "'" + flight_model.ETA + "'", flight_model.ATA == null ? "NULL" : "'" + flight_model.ATA + "'", flight_model.PREVIOUS_FLIGHT, flight_model.NEXT_FLIGHT, flight_model.ABNORMAL_STATUS, flight_model.AIRPORT1, flight_model.DORI1, flight_model.STD1, flight_model.AIRPORT2, flight_model.DORI2, flight_model.STA2 == null ? "NULL" : "'" + flight_model.STA2 + "'", flight_model.STD2 == null ? "NULL" : "'" + flight_model.STD2 + "'", flight_model.AIRPORT3, flight_model.DORI3, flight_model.STA3 == null ? "NULL" : "'" + flight_model.STA3 + "'", flight_model.STD3 == null ? "NULL" : "'" + flight_model.STD3 + "'", flight_model.AIRPORT4, flight_model.DORI4, flight_model.STA4, flight_model.CODE_SHARE1, flight_model.CODE_SHARE2, flight_model.CODE_SHARE3, flight_model.CODE_SHARE4, flight_model.ADD_TYPE, flight_model.Status_Code, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), flight_model.FLIGHT_NO);
                    db.ExecuteNonQuery(strSql);
                }
                else
                {
                    strSql = string.Format(@"INSERT INTO dbo.Flight_Dynamic(ID,FLG_IDU,OPERATION_DATE,FLIGHT_NO,AORD,DORI,TASK_CODE,TERMINAL_NO,AIRLINE_IATA,AIRCRAFT_TYPE_IATA,AC_REG_NO,SERVICE_CLASS,FLG_VIP,ORIGIN_AIRPORT_IATA,STD,ETD,ATD,DEST_AIRPORT_IATA,STA,ETA,ATA,PREVIOUS_FLIGHT,NEXT_FLIGHT,ABNORMAL_STATUS,AIRPORT1,DORI1,STD1,AIRPORT2,DORI2,STA2,STD2,AIRPORT3,DORI3,STA3,STD3,AIRPORT4,DORI4,STA4,CODE_SHARE1,CODE_SHARE2,CODE_SHARE3,CODE_SHARE4,ADD_TYPE,Status_Code) VALUES ({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}','{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}')", flight_model.ID, flight_model.FLG_IDU, flight_model.OPERATION_DATE, flight_model.FLIGHT_NO, flight_model.AORD, flight_model.DORI, flight_model.TASK_CODE, flight_model.TERMINAL_NO, flight_model.AIRLINE_IATA, flight_model.AIRCRAFT_TYPE_IATA, flight_model.AC_REG_NO, flight_model.SERVICE_CLASS, flight_model.FLG_VIP, flight_model.ORIGIN_AIRPORT_IATA, flight_model.STD, flight_model.ETD == null ? "NULL" : "'" + flight_model.ETD + "'", flight_model.ATD == null ? "NULL" : "'" + flight_model.ATD + "'", flight_model.DEST_AIRPORT_IATA, flight_model.STA, flight_model.ETA == null ? "NULL" : "'" + flight_model.ETA + "'", flight_model.ATA == null ? "NULL" : "'" + flight_model.ATA + "'", flight_model.PREVIOUS_FLIGHT, flight_model.NEXT_FLIGHT, flight_model.ABNORMAL_STATUS, flight_model.AIRPORT1, flight_model.DORI1, flight_model.STD1, flight_model.AIRPORT2, flight_model.DORI2, flight_model.STA2 == null ? "NULL" : "'" + flight_model.STA2 + "'", flight_model.STD2 == null ? "NULL" : "'" + flight_model.STD2 + "'", flight_model.AIRPORT3, flight_model.DORI3, flight_model.STA3 == null ? "NULL" : "'" + flight_model.STA3 + "'", flight_model.STD3 == null ? "NULL" : "'" + flight_model.STD3 + "'", flight_model.AIRPORT4, flight_model.DORI4, flight_model.STA4, flight_model.CODE_SHARE1, flight_model.CODE_SHARE2, flight_model.CODE_SHARE3, flight_model.CODE_SHARE4, flight_model.ADD_TYPE, flight_model.Status_Code);
                    db.ExecuteNonQuery(strSql);
                }
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 更新时间表i_fids_event中状态为已处理
        /// </summary>
        /// <param name="associate_id"></param>
        public static void UpdateEventStatus(string s_id)
        {
            string sql = "update i_fids_event set is_processed = 'Y' where id = " + s_id ;
            aodb.ExecuteNonQuery(sql);
        }

        #endregion

        #region 更新基础信息删除状态
        public static int UpdateFLG_DELETED(string tb_val,string tb_id,string id)
        {
            int count = 2;
            string strSql = string.Format(@"delete from "+tb_val+" where "+tb_id+"='{0}'", id);
            db.ExecuteNonQuery(strSql);
            return count;
        }

        #endregion
    }
}
