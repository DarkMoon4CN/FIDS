using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Utils
{
    public class ExcelHelper
    {
        public static DataTable ExcelToDataTable(string fileName,FileStream fs)
        {
            DataTable dt = new DataTable();
            IWorkbook wk = null;
            if (fileName.IndexOf(".xlsx") != -1)
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            else
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            fs.Close();
            ISheet sheet = wk.GetSheetAt(0);

            return dt;
        }

        public static DataTable ExcelToDataTable(ISheet sheet)
        {
            return null;
        }


        public static DataTable UltraExcelToDataTable(string fileName, Stream fs)
        {
            DataTable dt = new DataTable();
            IWorkbook wk = null;
            if (fileName.IndexOf(".xlsx") != -1)
            {
                //把xlsx文件中的数据写入wk中
                wk = new XSSFWorkbook(fs);
            }
            else
            {
                //把xls文件中的数据写入wk中
                wk = new HSSFWorkbook(fs);
            }
            fs.Close();
            ISheet sheet = wk.GetSheetAt(0);
            dt = UltraExcelToDataTable(sheet);
            return dt;
        }

        public static DataTable UltraExcelToDataTable(ISheet sheet)
        {
            DataTable dt = new DataTable();
            string FLIGHT_Date = string.Empty;
            bool isCreated = false;
            for (int i = 0; i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                //判定是否有时间点,查询
                if (row != null && row.Cells.Count > 0)
                {
                    short fcNum = row.FirstCellNum;
                    if (fcNum > 0)
                    {
                        //抓取时间
                        var cell = row.GetCell(row.FirstCellNum);
                        if (cell != null)
                        {
                            string value = cell.StringCellValue;
                            //字符串是否有括号
                            if (value.IndexOf("(") > -1 && value.IndexOf(")") > -1)
                            {
                                value = value.TrimStart('(');
                                FLIGHT_Date = value.TrimEnd(')');
                            }
                        }
                    }
                    else//数据主体
                    {
                        if(!isCreated)
                        {
                            dt.Columns.Add("ID",typeof(string));
                            dt.Columns.Add("A_FLIGHT_NO",typeof(string));//进港航班
                            dt.Columns.Add("A_TASK_CODE", typeof(string));//任务代码
                            dt.Columns.Add("A_AIRCRAFT_TYPE_IATA", typeof(string));//机型
                            dt.Columns.Add("A_AC_REG_NO", typeof(string));//机号
                            dt.Columns.Add("A_ORIGIN_AIRPORT_IATA", typeof(string));//起飞代码
                            dt.Columns.Add("A_STA", typeof(string));//计划到港对应 到港预飞
                            dt.Columns.Add("A_ETA", typeof(string));//预计到港
                            dt.Columns.Add("A_DEST_AIRPORT_IATA", typeof(string));//本场
                            
                           
                            dt.Columns.Add("D_FLIGHT_NO", typeof(string));//离港航班
                            dt.Columns.Add("D_TASK_CODE", typeof(string));//任务代码
                            dt.Columns.Add("D_AIRCRAFT_TYPE_IATA", typeof(string));//机型
                            dt.Columns.Add("D_AC_REG_NO", typeof(string));//机号
                            dt.Columns.Add("D_DEST_AIRPORT_IATA", typeof(string));//落地代码
                            dt.Columns.Add("D_STD", typeof(string));//计划离港
                            dt.Columns.Add("D_ETD", typeof(string));//预计离港
                            dt.Columns.Add("OPERATION_DATE", typeof(string));//运营日
                            isCreated = !isCreated;
                        }
                        //判定是否是列头
                        int numFlag = 0;
                        var isNum = int.TryParse(row.GetCell(0).ToString(), out numFlag);
                        if (!isNum)
                        {
                            continue;
                        }
                        var lcNum= row.LastCellNum;
                        var dataRow = dt.NewRow();
                        int m = 0;
                        for (int j = 0; j < lcNum; j++)
                        {
                            if (row.GetCell(j) != null 
                                     &&!string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                row.GetCell(j).SetCellType(CellType.STRING);
                                dataRow[m] = row.GetCell(j).StringCellValue;
                                m++;
                            }
                        }
                        dataRow["OPERATION_DATE"] = FLIGHT_Date;
                        dt.Rows.Add(dataRow);
                    }
                }
            }
            //进行DataTable里的时间字段进行调整
            foreach (DataRow dr in dt.Rows)
            {
                string A_STA=dr["A_STA"].ToString();
                A_STA= FLIGHT_Date+" "+A_STA.Substring(0, 2) + ":" + A_STA.Substring(2, 2)+":00";
                dr["A_STA"] = A_STA;

                string A_ETA = dr["A_ETA"].ToString();
                A_ETA = FLIGHT_Date + " " + A_ETA.Substring(0, 2) + ":" + A_ETA.Substring(2, 2) + ":00";
                dr["A_ETA"] = A_ETA;

                string D_STD = dr["D_STD"].ToString();
                D_STD = FLIGHT_Date + " " + D_STD.Substring(0, 2) + ":" + D_STD.Substring(2, 2) + ":00";
                dr["D_STD"] = D_STD;

                string D_ETD = dr["D_ETD"].ToString();
                D_ETD = FLIGHT_Date + " " + D_ETD.Substring(0, 2) + ":" + D_ETD.Substring(2, 2) + ":00";
                dr["D_ETD"] = D_ETD;
            }
            dt.AcceptChanges();
            return dt;
        }


    }
}
