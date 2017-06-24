using System;
using System.Data;
using System.Web.Mvc;
using CATC.FIDS.DAL.SynData;
using CATC.FIDS.Models;
using CATC.FIDS.Model;

namespace CATC.SynAodb.Controllers
{
    public class AodbSynDataController : Controller
    {
        CATC_FIDS_DBEntities ef = new CATC_FIDS_DBEntities();

        // GET: AodbSynData
        public ActionResult Index()
        {
            return View();
        }

        string strValue = "1";
        public string AutoSynData(string isInit = "")
        {
            if (string.IsNullOrEmpty(isInit))
            {
                strValue = "参数不全";
            }
            else
            {
                if (isInit == "n")
                {
                    EventChangeOper();
                }
                else if(isInit == "y")
                {
                    InitSynData();
                }
            }
            return strValue;
        }

        /// <summary>
        /// 初始化基础信息
        /// </summary>
        private void InitSynData()
        {
            DataSet ds_airc = SynAodbDAL.GetAircraft();
            DataSet ds_airl = SynAodbDAL.GetAirline();
            DataSet ds_airtype = SynAodbDAL.GetAircraftType();
            DataSet ds_airport = SynAodbDAL.GetAirport();
            DataSet ds_alliance = SynAodbDAL.GetAlliance();
            DataSet ds_city = SynAodbDAL.GetCity();
            DataSet ds_country = SynAodbDAL.GetCountry();
            DataSet ds_delcode = SynAodbDAL.GetDelayCode();
            DataSet ds_prov = SynAodbDAL.GetProvince();
            DataSet ds_task = SynAodbDAL.GetTaskCode();
            DataSet ds_airlsub = SynAodbDAL.GetAirlineSub();
            DataSet ds_gate = SynAodbDAL.GetGate();
            DataSet ds_bagage = SynAodbDAL.GetBagage();
            DataSet ds_checkin = SynAodbDAL.GetCheckin();
            int result = SynAodbDAL.SaveBasicsData(ds_airc,ds_airl,ds_airtype,ds_airport,ds_alliance,ds_city,ds_country,ds_delcode,ds_prov,ds_task,ds_airlsub,ds_gate, ds_bagage, ds_checkin);
            if (result == -1)
            {
                strValue = "初始化失败";
            }
            else
            {
                strValue = "初始化成功";
                L_Operation_Log sysLog = new L_Operation_Log();
                sysLog.LogTime = DateTime.Now;
                sysLog.LogType = (int)OperationLogEnum.系统日志;
                sysLog.ModuleType = (int)OperationLogModuleEnum.基础数据导入;
                sysLog.ModuleValue = "1";
                sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                ef.L_Operation_Log.Add(sysLog);
                ef.SaveChanges();
            }
        }

        /// <summary>
        /// 检查AODB事件表，并定时写入FIDS库
        /// </summary>
        private void EventChangeOper()

        {
            int? i_null = null;
            DateTime? dt = null;
            //取当前未处理事件
            DataSet ds = SynAodbDAL.GetEvent();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    bool isEdit = true;
                    int result = 0;
                    string module_val = "";
                    string str_id = ds.Tables[0].Rows[i]["id"].ToString();
                    string event_code = ds.Tables[0].Rows[i]["event_code"].ToString();
                    string associate_id = ds.Tables[0].Rows[i]["associate_id"].ToString();
                    
                    #region 基础数据变更
                    if (event_code == "1000")
                    {
                        DataSet ds_basic = SynAodbDAL.GetBasic_chg(associate_id);
                        if (ds_basic.Tables[0].Rows.Count > 0)
                        {
                            string table_name = ds_basic.Tables[0].Rows[0]["table_name"].ToString();
                            string flg_idu = ds_basic.Tables[0].Rows[0]["flg_idu"].ToString();
                            string pk_name = ds_basic.Tables[0].Rows[0]["pk_name"].ToString();
                            string pk_value = ds_basic.Tables[0].Rows[0]["pk_value"].ToString().Replace("[", "").Replace("]", "");
                            DataSet ds_b = SynAodbDAL.GetBasicData(table_name, pk_name, pk_value);
                            if (ds_b.Tables[0].Rows.Count > 0)
                            {
                                if (table_name == "SAIRCRAFT")
                                {
                                    F_Aircraft airc_model = new F_Aircraft();
                                    airc_model.AC_REG_NO = ds_b.Tables[0].Rows[0]["AC_REG_NO"].ToString().Replace("'", "").Trim();
                                    airc_model.AC_TYPE_IATA = ds_b.Tables[0].Rows[0]["AC_TYPE_IATA"].ToString().Replace("'", "").Trim();
                                    airc_model.AIRLINE_IATA = ds_b.Tables[0].Rows[0]["AIRLINE_IATA"].ToString().Replace("'", "").Trim();
                                    airc_model.SUBAIRLINE_ID = Convert.ToInt32(ds_b.Tables[0].Rows[0]["SUBAIRLINE_ID"].ToString().Replace("'", "").Trim());
                                    airc_model.FLG_DELETED = ds_b.Tables[0].Rows[0]["FLG_DELETED"].ToString().Replace("'", "").Trim();
                                    airc_model.EXT_CODE = ds_b.Tables[0].Rows[0]["EXT_CODE"].ToString().Replace("'", "").Trim();
                                    module_val = airc_model.AC_REG_NO;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Aircraft";
                                        string tb_id = "AC_REG_NO";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateAircraft(airc_model);
                                    }
                                }
                                else if (table_name == "SAIRCRAFTTYPE")
                                {
                                    F_AircraftType airtype_model = new F_AircraftType();
                                    airtype_model.iataCode = ds_b.Tables[0].Rows[0]["aircraft_type_iata"].ToString().Replace("'", "").Trim();
                                    airtype_model.icaoCode = ds_b.Tables[0].Rows[0]["ac_type_icao"].ToString().Replace("'", "").Trim();
                                    airtype_model.name_chinese = ds_b.Tables[0].Rows[0]["type_chinese"].ToString().Replace("'", "").Trim();
                                    airtype_model.name_english = ds_b.Tables[0].Rows[0]["type_english"].ToString().Replace("'", "").Trim();
                                    module_val = airtype_model.iataCode;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_AircraftType";
                                        string tb_id = "iataCode";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateAircraftType(airtype_model);
                                    }
                                }
                                else if (table_name == "SAIRLINE")
                                {
                                    F_Airline airline_model = new F_Airline();
                                    airline_model.Airline_IATA = ds_b.Tables[0].Rows[0]["Airline_IATA"].ToString().Replace("'", "").Trim();
                                    airline_model.Airline_ICAO = ds_b.Tables[0].Rows[0]["Airline_ICAO"].ToString().Replace("'", "").Trim();
                                    airline_model.Short_Name = ds_b.Tables[0].Rows[0]["Short_Name"].ToString().Replace("'", "").Trim();
                                    airline_model.Host_AirPort_IATA = ds_b.Tables[0].Rows[0]["Host_AirPort_IATA"].ToString().Replace("'", "").Trim();
                                    airline_model.DORI = ds_b.Tables[0].Rows[0]["DORI"].ToString().Replace("'", "").Trim();
                                    airline_model.NAME_ENGLISH = ds_b.Tables[0].Rows[0]["NAME_ENGLISH"].ToString().Replace("'", "").Trim();
                                    airline_model.NAME_CHINESE = ds_b.Tables[0].Rows[0]["NAME_CHINESE"].ToString().Replace("'", "").Trim();
                                    airline_model.ALLIANCE_CODE = ds_b.Tables[0].Rows[0]["ALLIANCE_CODE"].ToString().Replace("'", "").Trim();
                                    module_val = airline_model.Airline_IATA;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Airline";
                                        string tb_id = "Airline_IATA";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateAirline(airline_model);
                                    }
                                }
                                else if (table_name == "SAIRPORT")
                                {
                                    F_Airport airp_model = new F_Airport();
                                    airp_model.AIRPORT_IATA = ds_b.Tables[0].Rows[0]["airport_iata"].ToString().Replace("'", "").Trim();
                                    airp_model.AIRPORT_ICAO = ds_b.Tables[0].Rows[0]["airport_icao"].ToString().Replace("'", "").Trim();
                                    airp_model.SHORT_NAME = ds_b.Tables[0].Rows[0]["airport_short_name"].ToString().Replace("'", "").Trim();
                                    airp_model.CITY_IATA = ds_b.Tables[0].Rows[0]["city_iata"].ToString().Replace("'", "").Trim();
                                    airp_model.DORI = ds_b.Tables[0].Rows[0]["dori"].ToString().Replace("'", "").Trim();
                                    airp_model.NAME_CHINESE = ds_b.Tables[0].Rows[0]["name_chinese"].ToString().Replace("'", "").Trim();
                                    airp_model.NAME_ENGLISH = ds_b.Tables[0].Rows[0]["name_english"].ToString().Replace("'", "").Trim();
                                    airp_model.FLG_DELETED = ds_b.Tables[0].Rows[0]["flg_deleted"].ToString().Replace("'", "").Trim();
                                    airp_model.REGION_CODE = ds_b.Tables[0].Rows[0]["region_code"].ToString().Replace("'", "").Trim();
                                    module_val = airp_model.AIRPORT_IATA;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Airport";
                                        string tb_id = "AIRPORT_IATA";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateAirport(airp_model);
                                    }

                                }
                                else if (table_name == "SALLIANCE")
                                {
                                    F_Alliance all_model = new F_Alliance();
                                    all_model.ALLIANCE_NAME = ds_b.Tables[0].Rows[0]["alliance_code"].ToString().Replace("'", "").Trim();
                                    all_model.NAME_CHINESE = ds_b.Tables[0].Rows[0]["name_chinese"].ToString().Replace("'", "").Trim();
                                    all_model.NAME_ENGLISH = ds_b.Tables[0].Rows[0]["name_english"].ToString().Replace("'", "").Trim();
                                    all_model.REMARK = ds_b.Tables[0].Rows[0]["remark"].ToString().Replace("'", "").Trim();
                                    all_model.FLG_DELETED = ds_b.Tables[0].Rows[0]["flg_deleted"].ToString().Replace("'", "").Trim();
                                    module_val = all_model.ALLIANCE_NAME;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Alliance";
                                        string tb_id = "ALLIANCE_NAME";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateAlliance(all_model);
                                    }

                                }
                                else if (table_name == "SCITY")
                                {
                                    F_City city_model = new F_City();
                                    city_model.City_IATA = ds_b.Tables[0].Rows[0]["city_iata"].ToString().Replace("'", "").Trim();
                                    city_model.Country_IATA = ds_b.Tables[0].Rows[0]["country_iata"].ToString().Replace("'", "").Trim();
                                    city_model.City_ICAO = ds_b.Tables[0].Rows[0]["city_icao"].ToString().Replace("'", "").Trim();
                                    city_model.Name_Chinese = ds_b.Tables[0].Rows[0]["name_chinese"].ToString().Replace("'", "").Trim();
                                    city_model.Name_English = ds_b.Tables[0].Rows[0]["name_english"].ToString().Replace("'", "").Trim();
                                    city_model.Short_Chinese = ds_b.Tables[0].Rows[0]["short_chinese"].ToString().Replace("'", "").Trim();
                                    city_model.Province_IS = Convert.ToInt32(ds_b.Tables[0].Rows[0]["province_id"].ToString());
                                    city_model.FLG_Deleted = ds_b.Tables[0].Rows[0]["flg_deleted"].ToString().Replace("'", "").Trim();
                                    module_val = city_model.City_IATA;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_City";
                                        string tb_id = "City_IATA";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateCity(city_model);
                                    }

                                }
                                else if (table_name == "SCOUNTRY")
                                {
                                    F_Country coun_model = new F_Country();
                                    coun_model.Country_IATA = ds_b.Tables[0].Rows[0]["country_iata"].ToString().Replace("'", "").Trim();
                                    coun_model.Country_ICAO = ds_b.Tables[0].Rows[0]["country_icao"].ToString().Replace("'", "").Trim();
                                    coun_model.Name_Chinese = ds_b.Tables[0].Rows[0]["name_chinese"].ToString().Replace("'", "").Trim();
                                    coun_model.Name_English = ds_b.Tables[0].Rows[0]["name_english"].ToString().Replace("'", "").Trim();
                                    coun_model.FLG_Deleted = ds_b.Tables[0].Rows[0]["flg_deleted"].ToString().Replace("'", "").Trim();
                                    module_val = coun_model.Country_IATA;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Country";
                                        string tb_id = "Country_IATA";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateCountry(coun_model);
                                    }

                                }
                                else if (table_name == "SDELAYCODE")
                                {
                                    F_DelayCode del_model = new F_DelayCode();
                                    del_model.Delay_Code = ds_b.Tables[0].Rows[0]["delay_code"].ToString().Replace("'", "").Trim();
                                    del_model.Type = ds_b.Tables[0].Rows[0]["type"].ToString().Replace("'", "").Trim();
                                    del_model.Code_Chinese = ds_b.Tables[0].Rows[0]["code_chinese"].ToString().Replace("'", "").Trim();
                                    del_model.Code_English = ds_b.Tables[0].Rows[0]["code_english"].ToString().Replace("'", "").Trim();
                                    del_model.Description = ds_b.Tables[0].Rows[0]["description"].ToString().Replace("'", "").Trim();
                                    del_model.FLG_Deleted = ds_b.Tables[0].Rows[0]["flg_deleted"].ToString().Replace("'", "").Trim();
                                    module_val = del_model.Delay_Code;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_DelayCode";
                                        string tb_id = "Delay_Code";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateDelayCode(del_model);
                                    }

                                }
                                else if (table_name == "SPROVINCE")
                                {
                                    F_Province prov_mdoel = new F_Province();
                                    prov_mdoel.Province_ID = Convert.ToInt32(ds_b.Tables[0].Rows[0]["province_id"].ToString());
                                    prov_mdoel.Name_Chinese = ds_b.Tables[0].Rows[0]["province_cn"].ToString().Replace("'", "").Trim();
                                    prov_mdoel.Name_English = ds_b.Tables[0].Rows[0]["province_en"].ToString().Replace("'", "").Trim();
                                    prov_mdoel.Short_Name = ds_b.Tables[0].Rows[0]["province_short_name"].ToString().Replace("'", "").Trim();
                                    prov_mdoel.DORI = ds_b.Tables[0].Rows[0]["dori"].ToString().Replace("'", "").Trim();
                                    module_val = prov_mdoel.Province_ID.ToString();
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Province";
                                        string tb_id = "Province_ID";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateProvince(prov_mdoel);
                                    }

                                }
                                else if (table_name == "STASKCODE")
                                {
                                    F_TaskCode task_mdoel = new F_TaskCode();
                                    task_mdoel.Task_Code = ds_b.Tables[0].Rows[0]["task_code"].ToString();
                                    task_mdoel.Name_Chinese = ds_b.Tables[0].Rows[0]["task_chinese"].ToString().Replace("'", "").Trim();
                                    task_mdoel.Name_English = ds_b.Tables[0].Rows[0]["task_english"].ToString().Replace("'", "").Trim();
                                    task_mdoel.Description = ds_b.Tables[0].Rows[0]["description"].ToString().Replace("'", "").Trim();
                                    task_mdoel.FLG_Deleted = ds_b.Tables[0].Rows[0]["flg_deleted"].ToString().Replace("'", "").Trim();
                                    module_val = task_mdoel.Task_Code;
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_TaskCode";
                                        string tb_id = "Task_Code";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateTaskCode(task_mdoel);
                                    }

                                }
                                else if (table_name == "SSUBAIRLINE")
                                {
                                    F_Airline_Sub airsub_model = new F_Airline_Sub();
                                    airsub_model.ID = Convert.ToInt32(ds_b.Tables[0].Rows[0]["id"].ToString());
                                    airsub_model.Parent_Airline = ds_b.Tables[0].Rows[0]["parent_airline"].ToString().Replace("'", "").Trim();
                                    airsub_model.Subairline_Name = ds_b.Tables[0].Rows[0]["subairline_name"].ToString().Replace("'", "").Trim();
                                    module_val = airsub_model.ID.ToString();
                                    if (flg_idu == "D")
                                    {
                                        string tb_val = "F_Airline_Sub";
                                        string tb_id = "ID";
                                        result = SynAodbDAL.UpdateFLG_DELETED(tb_val, tb_id, module_val);
                                    }
                                    else
                                    {
                                        result = SynAodbDAL.UpdateAirlineSub(airsub_model);
                                    }
                                }
                                else if (table_name == "FGATE")
                                {
                                    R_Facility gate_model = new R_Facility();
                                    gate_model.Aodb_Facility_ID = ds_b.Tables[0].Rows[0]["facility_id"].ToString().Replace("'", "").Trim();
                                    gate_model.Display_Symbol = ds_b.Tables[0].Rows[0]["display_symbol"].ToString().Replace("'", "").Trim();
                                    gate_model.Terminal_NO = ds_b.Tables[0].Rows[0]["terminal_no"].ToString().Replace("'", "").Trim();
                                    gate_model.DORI = ds_b.Tables[0].Rows[0]["dori"].ToString().Replace("'", "").Trim();
                                    gate_model.Description = ds_b.Tables[0].Rows[0]["description"].ToString().Replace("'", "").Trim();
                                    gate_model.Status = ds_b.Tables[0].Rows[0]["status"].ToString().Replace("'", "").Trim();
                                    gate_model.Status_Timestamp = string.IsNullOrEmpty(ds_b.Tables[0].Rows[0]["status_timestamp"].ToString()) ? dt : Convert.ToDateTime(ds_b.Tables[0].Rows[0]["status_timestamp"].ToString().Replace("'", "").Trim());
                                    gate_model.Status_Remark = ds_b.Tables[0].Rows[0]["status_remark"].ToString().Replace("'", "").Trim();
                                    gate_model.Recorder_ID = string.IsNullOrEmpty(ds_b.Tables[0].Rows[0]["recorder_id"].ToString()) ? 0 : Convert.ToInt32(ds_b.Tables[0].Rows[0]["recorder_id"].ToString().Replace("'", "").Trim());
                                    SynAodbDAL.UpdateFgate(gate_model);
                                }
                                //---写入日志---//
                                L_Operation_Log sysLog = new L_Operation_Log();
                                if (result == 1)
                                {
                                    sysLog.ModuleType = (int)OperationLogModuleEnum.基础数据编辑;
                                }
                                else if (result == 2)
                                {
                                    sysLog.ModuleType = (int)OperationLogModuleEnum.基础数据删除;
                                }
                                else
                                {
                                    sysLog.ModuleType = (int)OperationLogModuleEnum.基础数据增加;
                                }
                                sysLog.LogTime = DateTime.Now;
                                sysLog.LogType = (int)OperationLogEnum.系统日志;
                                sysLog.ModuleValue = module_val;
                                sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                                ef.L_Operation_Log.Add(sysLog);
                                ef.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region 航班数据初始化
                    else if (event_code == "4100")
                    {
                        //先备份
                        SynAodbDAL.BackUpFlight_Dynamic();
                        DataSet ds_all = SynAodbDAL.GetAllFlight_Curday();
                        int count = ds_all.Tables[0].Rows.Count;
                        if (count > 0)
                        {
                            for (int j = 0; j < count; j++)
                            {
                                Flight_Dynamic flight_model = new Flight_Dynamic();
                                flight_model.ID = Convert.ToInt32(ds_all.Tables[0].Rows[j]["id"].ToString());
                                flight_model.FLG_IDU = ds_all.Tables[0].Rows[j]["flg_idu"].ToString().Replace("'", "").Trim();
                                flight_model.OPERATION_DATE = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["operation_date"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["operation_date"].ToString());
                                flight_model.FLIGHT_NO = ds_all.Tables[0].Rows[j]["flight_no"].ToString().Replace("'", "").Trim();
                                flight_model.AORD = ds_all.Tables[0].Rows[j]["aord"].ToString().Replace("'", "").Trim();
                                flight_model.DORI = ds_all.Tables[0].Rows[j]["dori"].ToString().Replace("'", "").Trim();
                                flight_model.TASK_CODE = ds_all.Tables[0].Rows[j]["task_code"].ToString().Replace("'", "").Trim();
                                flight_model.TERMINAL_NO = ds_all.Tables[0].Rows[j]["terminal_no"].ToString().Replace("'", "").Trim();
                                flight_model.AIRLINE_IATA = ds_all.Tables[0].Rows[j]["airline_iata"].ToString().Replace("'", "").Trim();
                                flight_model.AIRCRAFT_TYPE_IATA = ds_all.Tables[0].Rows[j]["aircraft_type_iata"].ToString().Replace("'", "").Trim();
                                flight_model.AC_REG_NO = ds_all.Tables[0].Rows[j]["ac_reg_no"].ToString().Replace("'", "").Trim();
                                flight_model.SERVICE_CLASS = ds_all.Tables[0].Rows[j]["service_class"].ToString().Replace("'", "").Trim();
                                flight_model.FLG_VIP = ds_all.Tables[0].Rows[j]["flg_vip"].ToString().Replace("'", "").Trim();
                                flight_model.ORIGIN_AIRPORT_IATA = ds_all.Tables[0].Rows[j]["origin_airport_iata"].ToString().Replace("'", "").Trim();
                                flight_model.STD = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["std"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["std"].ToString());
                                flight_model.ETD = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["etd"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["etd"].ToString());
                                flight_model.ATD = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["atd"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["atd"].ToString());
                                flight_model.DEST_AIRPORT_IATA = ds_all.Tables[0].Rows[j]["dest_airport_iata"].ToString().Replace("'", "").Trim();
                                flight_model.STA = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["sta"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["sta"].ToString());
                                flight_model.ETA = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["eta"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["eta"].ToString());
                                flight_model.ATA = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["ata"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["ata"].ToString());
                                flight_model.PREVIOUS_FLIGHT = ds_all.Tables[0].Rows[j]["previous_flight"].ToString().Replace("'", "").Trim();
                                flight_model.NEXT_FLIGHT = ds_all.Tables[0].Rows[j]["next_flight"].ToString().Replace("'", "").Trim();
                                flight_model.ABNORMAL_STATUS = ds_all.Tables[0].Rows[j]["abnormal_status"].ToString().Replace("'", "").Trim();
                                flight_model.AIRPORT1 = ds_all.Tables[0].Rows[j]["airport1"].ToString().Replace("'", "").Trim();
                                flight_model.DORI1 = ds_all.Tables[0].Rows[j]["dori1"].ToString().Replace("'", "").Trim();
                                flight_model.STD1 = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["std1"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["std1"].ToString());
                                flight_model.AIRPORT2 = ds_all.Tables[0].Rows[j]["airport2"].ToString().Replace("'", "").Trim();
                                flight_model.DORI2 = ds_all.Tables[0].Rows[j]["dori2"].ToString().Replace("'", "").Trim();
                                flight_model.STA2 = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["sta2"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["sta2"].ToString());
                                flight_model.STD2 = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["std2"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["std2"].ToString());
                                flight_model.AIRPORT3 = ds_all.Tables[0].Rows[j]["airport3"].ToString().Replace("'", "").Trim();
                                flight_model.DORI3 = ds_all.Tables[0].Rows[j]["dori3"].ToString().Replace("'", "").Trim();
                                flight_model.STA3 = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["sta3"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["sta3"].ToString());
                                flight_model.STD3 = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["std3"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["std3"].ToString());
                                flight_model.AIRPORT4 = ds_all.Tables[0].Rows[j]["airport7"].ToString().Replace("'", "").Trim();
                                flight_model.STA4 = string.IsNullOrEmpty(ds_all.Tables[0].Rows[j]["sta7"].ToString()) ? dt : Convert.ToDateTime(ds_all.Tables[0].Rows[j]["sta7"].ToString());
                                flight_model.CODE_SHARE1 = ds_all.Tables[0].Rows[j]["code_share1"].ToString().Replace("'", "").Trim();
                                flight_model.CODE_SHARE2 = ds_all.Tables[0].Rows[j]["code_share2"].ToString().Replace("'", "").Trim();
                                flight_model.CODE_SHARE3 = ds_all.Tables[0].Rows[j]["code_share3"].ToString().Replace("'", "").Trim();
                                flight_model.CODE_SHARE4 = ds_all.Tables[0].Rows[j]["code_share4"].ToString().Replace("'", "").Trim();
                                flight_model.Status_Code = ds_all.Tables[0].Rows[j]["abnormal_status"].ToString().Replace("'", "").Trim();
                                flight_model.ADD_TYPE = 0;
                                SynAodbDAL.InsertFlight_Curday(flight_model);
                            }
                            //---写入日志---//
                            L_Operation_Log sysLog = new L_Operation_Log();
                            sysLog.ModuleType = (int)OperationLogModuleEnum.航班数据导入;
                            sysLog.LogTime = DateTime.Now;
                            sysLog.LogType = (int)OperationLogEnum.系统日志;
                            sysLog.ModuleValue = "88888";
                            sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                            ef.L_Operation_Log.Add(sysLog);
                            ef.SaveChanges();
                        }

                    }
                    #endregion

                    #region 资源分配数据初始化 
                    else if (event_code == "4101")
                    {
                        //资源分配数据初始化
                        DataSet ds_fresAll = SynAodbDAL.GetAllFlight_res();
                        int num = ds_fresAll.Tables[0].Rows.Count;
                        if (num > 0)
                        {
                            for (int k = 0; k < num; k++)
                            {
                                Flight_Resource_Allocation flight_rmodel = new Flight_Resource_Allocation();
                                flight_rmodel.ID = Convert.ToInt32(ds_fresAll.Tables[0].Rows[k]["id"].ToString());
                                flight_rmodel.OPERATION_DATE = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["operation_date"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["operation_date"].ToString());
                                flight_rmodel.FLIGHT_NO = ds_fresAll.Tables[0].Rows[k]["flight_no"].ToString().Replace("'", "").Trim();
                                flight_rmodel.AORD = ds_fresAll.Tables[0].Rows[k]["aord"].ToString().Replace("'", "").Trim();
                                //离港
                                if(flight_rmodel.AORD=="D")
                                {
                                    string sched_id = ds_fresAll.Tables[0].Rows[k]["gate_sched_id"].ToString().Replace("'", "").Trim();
                                    DataSet ds_schedid = SynAodbDAL.GetFacilityID(sched_id);
                                    if (ds_schedid.Tables[0].Rows.Count > 0)
                                    {
                                        flight_rmodel.SCHED_ID = string.IsNullOrEmpty(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString()) ? i_null : Convert.ToInt32(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString());
                                        flight_rmodel.ESTIMATE_ID = string.IsNullOrEmpty(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString()) ? i_null : Convert.ToInt32(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString());
                                        flight_rmodel.ACTUAL_ID = string.IsNullOrEmpty(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString()) ? i_null : Convert.ToInt32(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString());
                                    }
                                    else
                                    {
                                        isEdit = false;
                                        continue;
                                    }
                                    flight_rmodel.SCHED_START = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["gate_sched_start"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["gate_sched_start"].ToString());
                                    flight_rmodel.SCHED_END = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["gate_sched_end"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["gate_sched_end"].ToString());
                                    flight_rmodel.ESTIMATE_START = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["gate_estimate_start"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["gate_estimate_start"].ToString());
                                    flight_rmodel.ESTIMATE_END = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["gate_estimate_end"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["gate_estimate_end"].ToString());
                                    flight_rmodel.ACTUAL_START = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["gate_actual_start"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["gate_actual_start"].ToString());
                                    flight_rmodel.ACTUAL_END = string.IsNullOrEmpty(ds_fresAll.Tables[0].Rows[k]["gate_actual_end"].ToString()) ? dt : Convert.ToDateTime(ds_fresAll.Tables[0].Rows[k]["gate_actual_end"].ToString());
                                    flight_rmodel.FacilityType = 1;
                                    result = SynAodbDAL.InsertAllFlight_Res(flight_rmodel);
                                }
                                //进港
                                else if(flight_rmodel.AORD == "A")
                                {
                                    //后续处理
                                    continue;
                                }
                            }
                            //---写入日志---//
                            L_Operation_Log sysLog = new L_Operation_Log();
                            sysLog.ModuleType = (int)OperationLogModuleEnum.航班资源分配导入;
                            sysLog.LogTime = DateTime.Now;
                            sysLog.LogType = (int)OperationLogEnum.系统日志;
                            sysLog.ModuleValue = "88888";
                            sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                            ef.L_Operation_Log.Add(sysLog);
                            ef.SaveChanges();
                        }
                    }
                    #endregion

                    #region 航班资源分配数据变更 
                    else if (event_code == "12294")
                    {
                        DataSet ds_flight = SynAodbDAL.GetFlight_res(associate_id);
                        if (ds_flight.Tables[0].Rows.Count > 0)
                        {
                            Flight_Resource_Allocation flight_rmodel = new Flight_Resource_Allocation();
                            flight_rmodel.ID = Convert.ToInt32(ds_flight.Tables[0].Rows[0]["id"].ToString());
                            flight_rmodel.OPERATION_DATE = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["operation_date"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["operation_date"].ToString());
                            flight_rmodel.FLIGHT_NO = ds_flight.Tables[0].Rows[0]["flight_no"].ToString().Replace("'", "").Trim();
                            flight_rmodel.AORD = ds_flight.Tables[0].Rows[0]["aord"].ToString().Replace("'", "").Trim();

                            string sched_id = ds_flight.Tables[0].Rows[0]["gate_sched_id"].ToString().Replace("'", "").Trim();
                            DataSet ds_schedid = SynAodbDAL.GetFacilityID(sched_id);
                            if (ds_schedid.Tables[0].Rows.Count > 0)
                            {
                                flight_rmodel.SCHED_ID = string.IsNullOrEmpty(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString()) ? i_null : Convert.ToInt32(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString());
                                flight_rmodel.ESTIMATE_ID = string.IsNullOrEmpty(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString()) ? i_null : Convert.ToInt32(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString());
                                flight_rmodel.ACTUAL_ID = string.IsNullOrEmpty(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString()) ? i_null : Convert.ToInt32(ds_schedid.Tables[0].Rows[0]["Facility_ID"].ToString());
                            }
                            else
                            {
                                isEdit = false;
                                continue;
                            }
                            flight_rmodel.SCHED_START = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["gate_sched_start"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["gate_sched_start"].ToString());
                            flight_rmodel.SCHED_END = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["gate_sched_end"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["gate_sched_end"].ToString());
                            flight_rmodel.ESTIMATE_START = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["gate_estimate_start"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["gate_estimate_start"].ToString());
                            flight_rmodel.ESTIMATE_END = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["gate_estimate_end"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["gate_estimate_end"].ToString());
                            flight_rmodel.ACTUAL_START = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["gate_actual_start"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["gate_actual_start"].ToString());
                            flight_rmodel.ACTUAL_END = string.IsNullOrEmpty(ds_flight.Tables[0].Rows[0]["gate_actual_end"].ToString()) ? dt : Convert.ToDateTime(ds_flight.Tables[0].Rows[0]["gate_actual_end"].ToString());
                            flight_rmodel.FacilityType = 1;
                            result = SynAodbDAL.InsertFlight_Res(flight_rmodel);
                            //---写入日志---//
                            L_Operation_Log sysLog = new L_Operation_Log();
                            if (result == 1)
                            {
                                sysLog.ModuleType = (int)OperationLogModuleEnum.航班资源分配编辑;
                            }
                            else
                            {
                                sysLog.ModuleType = (int)OperationLogModuleEnum.航班资源分配增加;
                            }
                            sysLog.LogTime = DateTime.Now;
                            sysLog.LogType = (int)OperationLogEnum.系统日志;
                            sysLog.ModuleValue = flight_rmodel.FLIGHT_NO;
                            sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                            ef.L_Operation_Log.Add(sysLog);
                            ef.SaveChanges();
                        }
                    }
                    #endregion

                    #region 航班数据变更
                    else 
                    if(event_code == "4113"|| event_code == "4114"|| event_code == "4115" || event_code == "4116" || event_code == "4120" || event_code == "4129" || event_code == "4130" || event_code == "4131" || event_code == "4132" || event_code == "4133" || event_code == "4134" || event_code == "4135" || event_code == "4136" || event_code == "4138")
                    {
                        DataSet ds_flight_c = SynAodbDAL.GetFlight_Curday(associate_id);
                        Flight_Dynamic flight_model = new Flight_Dynamic();
                        flight_model.ID = Convert.ToInt32(ds_flight_c.Tables[0].Rows[0]["id"].ToString());
                        flight_model.FLG_IDU = ds_flight_c.Tables[0].Rows[0]["flg_idu"].ToString().Replace("'", "").Trim();
                        flight_model.OPERATION_DATE = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["operation_date"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["operation_date"].ToString());
                        flight_model.FLIGHT_NO = ds_flight_c.Tables[0].Rows[0]["flight_no"].ToString().Replace("'", "").Trim();
                        flight_model.AORD = ds_flight_c.Tables[0].Rows[0]["aord"].ToString().Replace("'", "").Trim();
                        flight_model.DORI = ds_flight_c.Tables[0].Rows[0]["dori"].ToString().Replace("'", "").Trim();
                        flight_model.TASK_CODE = ds_flight_c.Tables[0].Rows[0]["task_code"].ToString().Replace("'", "").Trim();
                        flight_model.TERMINAL_NO = ds_flight_c.Tables[0].Rows[0]["terminal_no"].ToString().Replace("'", "").Trim();
                        flight_model.AIRLINE_IATA = ds_flight_c.Tables[0].Rows[0]["airline_iata"].ToString().Replace("'", "").Trim();
                        flight_model.AIRCRAFT_TYPE_IATA = ds_flight_c.Tables[0].Rows[0]["aircraft_type_iata"].ToString().Replace("'", "").Trim();
                        flight_model.AC_REG_NO = ds_flight_c.Tables[0].Rows[0]["ac_reg_no"].ToString().Replace("'", "").Trim();
                        flight_model.SERVICE_CLASS = ds_flight_c.Tables[0].Rows[0]["service_class"].ToString().Replace("'", "").Trim();
                        flight_model.FLG_VIP = ds_flight_c.Tables[0].Rows[0]["flg_vip"].ToString().Replace("'", "").Trim();
                        flight_model.ORIGIN_AIRPORT_IATA = ds_flight_c.Tables[0].Rows[0]["origin_airport_iata"].ToString().Replace("'", "").Trim();
                        flight_model.STD = DateTimeNull(ds_flight_c.Tables[0].Rows[0]["std"]);
                        flight_model.ETD = DateTimeNull(ds_flight_c.Tables[0].Rows[0]["etd"]);
                        flight_model.ATD = DateTimeNull(ds_flight_c.Tables[0].Rows[0]["atd"]);
                        flight_model.DEST_AIRPORT_IATA = ds_flight_c.Tables[0].Rows[0]["dest_airport_iata"].ToString().Replace("'", "").Trim();
                        flight_model.STA = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["sta"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["sta"].ToString());
                        flight_model.ETA = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["eta"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["eta"].ToString());
                        flight_model.ATA = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["ata"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["ata"].ToString());
                        flight_model.PREVIOUS_FLIGHT = ds_flight_c.Tables[0].Rows[0]["previous_flight"].ToString().Replace("'", "").Trim();
                        flight_model.NEXT_FLIGHT = ds_flight_c.Tables[0].Rows[0]["next_flight"].ToString().Replace("'", "").Trim();
                        flight_model.ABNORMAL_STATUS = ds_flight_c.Tables[0].Rows[0]["abnormal_status"].ToString().Replace("'", "").Trim();
                        flight_model.AIRPORT1 = ds_flight_c.Tables[0].Rows[0]["airport1"].ToString().Replace("'", "").Trim();
                        flight_model.DORI1 = ds_flight_c.Tables[0].Rows[0]["dori1"].ToString().Replace("'", "").Trim();
                        flight_model.STD1 = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["std1"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["std1"].ToString());
                        flight_model.AIRPORT2 = ds_flight_c.Tables[0].Rows[0]["airport2"].ToString().Replace("'", "").Trim();
                        flight_model.DORI2 = ds_flight_c.Tables[0].Rows[0]["dori2"].ToString().Replace("'", "").Trim();
                        flight_model.STA2 = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["sta2"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["sta2"].ToString());
                        flight_model.STD2 = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["std2"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["std2"].ToString());
                        flight_model.AIRPORT3 = ds_flight_c.Tables[0].Rows[0]["airport3"].ToString().Replace("'", "").Trim();
                        flight_model.DORI3 = ds_flight_c.Tables[0].Rows[0]["dori3"].ToString().Replace("'", "").Trim();
                        flight_model.STA3 = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["sta3"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["sta3"].ToString());
                        flight_model.STD3 = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["std3"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["std3"].ToString());
                        flight_model.AIRPORT4 = ds_flight_c.Tables[0].Rows[0]["airport7"].ToString().Replace("'", "").Trim();
                        flight_model.STA4 = string.IsNullOrEmpty(ds_flight_c.Tables[0].Rows[0]["sta7"].ToString()) ? dt : Convert.ToDateTime(ds_flight_c.Tables[0].Rows[0]["sta7"].ToString());
                        flight_model.CODE_SHARE1 = ds_flight_c.Tables[0].Rows[0]["code_share1"].ToString().Replace("'", "").Trim();
                        flight_model.CODE_SHARE2 = ds_flight_c.Tables[0].Rows[0]["code_share2"].ToString().Replace("'", "").Trim();
                        flight_model.CODE_SHARE3 = ds_flight_c.Tables[0].Rows[0]["code_share3"].ToString().Replace("'", "").Trim();
                        flight_model.CODE_SHARE4 = ds_flight_c.Tables[0].Rows[0]["code_share4"].ToString().Replace("'", "").Trim();
                        flight_model.Status_Code = ds_flight_c.Tables[0].Rows[0]["abnormal_status"].ToString().Replace("'", "").Trim();
                        flight_model.ADD_TYPE = 0;
                        result = SynAodbDAL.InsertFlight_Curday(flight_model);
                        //---写入日志---//
                        L_Operation_Log sysLog = new L_Operation_Log();
                        if (result == 1)
                        {
                            sysLog.ModuleType = (int)OperationLogModuleEnum.航班数据编辑;
                        }
                        else
                        {
                            sysLog.ModuleType = (int)OperationLogModuleEnum.航班数据增加;
                        }
                        sysLog.LogTime = DateTime.Now;
                        sysLog.LogType = (int)OperationLogEnum.系统日志;
                        sysLog.ModuleValue = flight_model.FLIGHT_NO;
                        sysLog.LogLevel = (int)OperationLogLevelEnum.Info;
                        ef.L_Operation_Log.Add(sysLog);
                        ef.SaveChanges();
                    }
                    else
                    {
                        isEdit = false;
                    }
                    #endregion

                    if(isEdit)
                    {
                        //更新状态为已处理
                        SynAodbDAL.UpdateEventStatus(str_id);
                    } 
                }
            }
        }


        private DateTime? DateTimeNull(object a)
        {
            if (a.GetType() == typeof(DBNull))
            {
                return null;
            }
            else
            {
                return a.ToDateTime();
            }
        }
    }
}