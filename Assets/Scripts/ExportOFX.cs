using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEditor.UI;
using TMPro;

public class ExportOFX : MonoBehaviour
{
    [SerializeField]
    GameObject _controller;
    [SerializeField]
    string _outputName = "OutputOFX";
    string date = "";

    private string _brokerID;
    private string _accID;

    [SerializeField]
    TMP_Text _brokerText;
    [SerializeField]
    TMP_Text _accText;

    private void Start()
    {
        //Find current date
        date = System.DateTime.Now.ToString("yyyyMMdd");
    }

    public void ConvertToOFX(Dictionary<int, List<string>> input)
    {
        //Find original path
        OpenFile openScript = _controller.GetComponent<OpenFile>();
        string path = openScript.Path;
        string[] orPathArr = path.Split('/');
        string newPath = "";

        //Put new path in same folder as original path
        orPathArr[orPathArr.Length - 1] = $"{_outputName}.ofx";
        int i = 0;
        foreach (var word in orPathArr)
        {
            if (i != orPathArr.Length - 1)
            {
                newPath += orPathArr[i] + "/";
            }
            else
            {
                newPath += orPathArr[i];
            }
            

            i++;
        }

        //Overwrite file if exists already
        if(File.Exists(newPath))
        {
            File.Delete(newPath);
        }

        #region Write File
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(newPath, true);
        //WRITE HEADER
        writer.WriteLine("<?xml version=\"1.0\" encoding=\"US - ASCII\"?>");
        writer.WriteLine("<?OFX OFXHEADER=\"200\" VERSION=\"200\" SECURITY=\"NONE\" OLDFILEUID=\"NONE\" NEWFILEUID=\"NONE\"?>");

        //WRITE BODY
        writer.WriteLine("<OFX>");

        #region WRITE SIGN ON
        writer.WriteLine("<SIGNONMSGSRSV1>");
        writer.WriteLine("<SONRS>");

        #region WRITE SUCCESS MESSAGE
        writer.WriteLine("<STATUS>");
        writer.WriteLine("<CODE>0</CODE>");
        writer.WriteLine("<SEVERITY>INFO</SEVERITY>");
        writer.WriteLine("</STATUS>");
        #endregion

        #region WRITE DATE AND LANGUAGE
        writer.WriteLine($"<DTSERVER>{date}120000</DTSERVER>");
        writer.WriteLine("<LANGUAGE>ENG</LANGUAGE>");
        #endregion

        #region WRITE FINANCIAL INSTITUTIONS
        //writer.WriteLine("<FI>");
        //writer.WriteLine("<ORG>B1");
        //writer.WriteLine("<FID>10898");
        //writer.WriteLine("</FI>");
        //writer.WriteLine("<INTU.BID>10898");
        #endregion

        writer.WriteLine("</SONRS>");
        writer.WriteLine("</SIGNONMSGSRSV1>");
        #endregion

        #region WRITE BANKING
        //writer.WriteLine("<BANKMSGRSV1>");



        //writer.WriteLine("</BANKMSGRSV1>");
        #endregion

        #region WRITE CREDIT CARD
        //writer.WriteLine("<CREDITCARDMSGRSV1>");



        //writer.WriteLine("</CREDITCARDMSGRSV1>");
        #endregion

        #region WRITE INVESTMENT
        writer.WriteLine("<INVSTMTMSGRSV1>");
        writer.WriteLine("<INVSTMTTRNRS>");

        #region WRITE SUCCESS
        writer.WriteLine("<TRNUID>1001</TRNUID>");
        writer.WriteLine("<STATUS>");
        writer.WriteLine("<CODE>0</CODE>");
        writer.WriteLine("<SEVERITY>INFO</SEVERITY>");
        writer.WriteLine("</STATUS>");
        #endregion

        #region WRITE STOCK BUY
        
        writer.WriteLine("<INVSTMTRS>");
        #region ACC INFO
        //HEADER
        writer.WriteLine($"<DTASOF>{date}120000</DTASOF>");
        writer.WriteLine("<CURDEF>USD</CURDEF>");
        //BROKER ID AND ACC ID
        writer.WriteLine("<INVACCTFROM>");
        writer.WriteLine($"<BROKERID>{_brokerID}</BROKERID>");
        writer.WriteLine($"<ACCTID>{_accID}</ACCTID>");
        
        writer.WriteLine("</INVACCTFROM>");
        #endregion
        #region Transactions

        i = 0;
        string startDate = "";
        foreach (var value in input.Values)
        {
            if (i == 0)
            {
                startDate = ParseDate(value[0]);
            }
            i++;
        }
        writer.WriteLine($"<DTSTART>{startDate}");
        writer.WriteLine($"<DTEND>{date}");
        i = 0;
        foreach (var value in input.Values)
        {
            if (value[2] != "")
            {
                #region WRITE STOCK
                writer.WriteLine("<BUYSTOCK>");
                writer.WriteLine("<INVBUY>");
                writer.WriteLine("<INVTRAN>");
                writer.WriteLine($"<FITID>{value[1]}</FITID>");

                string dateNoTime;
                if (value[0] != "***END OF FILE***")
                {
                    dateNoTime = ParseDateNoTime(value[0]);
                }
                else
                {
                    dateNoTime = "20210101";
                }

                writer.WriteLine($"<DTTRADE>{dateNoTime}</DTTRADE>");
                writer.WriteLine($"<DTSETTLE>{dateNoTime}</DTSETTLE>");

                writer.WriteLine($"<MEMO>{value[2]}</MEMO>");
                writer.WriteLine("</INVTRAN>");

                writer.WriteLine("<SECID>");
                writer.WriteLine("<UNIQUEID>037833100</UNIQUEID>");
                writer.WriteLine("<UNIQUEIDTYPE>CUSIP</UNIQUEIDTYPE>");
                writer.WriteLine("</SECID>");

                writer.WriteLine($"<UNITS>{value[3]}</UNITS>");
                writer.WriteLine($"<UNITPRICE>{value[5]}</UNITPRICE>");
                writer.WriteLine($"<COMMISSION>{value[6]}</COMMISSION>");
                writer.WriteLine($"<TOTAL>{value[7]}</TOTAL>");
                writer.WriteLine("<SUBACCTSEC>CASH</SUBACCTSEC>");
                writer.WriteLine("<SUBACCTFUND>CASH</SUBACCTFUND>");
                writer.WriteLine("</INVBUY>");
                writer.WriteLine("<BUYTYPE>BUY</BUYTYPE>");
                writer.WriteLine("</BUYSTOCK>");
                #endregion
                print("WRITE COMPLETED: " + (i+1));
                i++;
            }
        }


        #endregion
        writer.WriteLine("<INVSTMTRS>");
        #endregion

        writer.WriteLine("</INVSTMTTRNRS>");
        writer.WriteLine("</INVSTMTMSGRSV1>");
        #endregion

        writer.WriteLine("</OFX>");
        writer.Close();

        #endregion


        Debug.Log("END");
    }

    public void UpdateInfo()
    {
        //Update vars based on text field input
        _brokerID = _brokerText.text;
        _accID = _accText.text;
    }

    string ParseDate(string dateInput)
    {
        string newDate = "";
        if (dateInput != "***END OF FILE***")
        {
            print(dateInput);
            
            //Format date into date+hour
            string[] dateChars = dateInput.Split('/');
            if (dateChars[2] == null || dateChars[1] == null || dateChars[0] == null)
            {
                newDate = "20210101120000";
            }
            else
            {
                newDate = $"{dateChars[2]}{dateChars[1]}{dateChars[0]}120000";
            }
            
            //string newDate = $"{dateChars[2]}{dateChars[1]}{dateChars[0]}120000";
        }

        return newDate;
    }
    string ParseDateNoTime(string dateInput)
    {
        //Format Date
        string[] dateChars = dateInput.Split('/');
        string newDate;
        if (dateChars[2] == null || dateChars[1] == null || dateChars[0] == null)
        {
            newDate = "20210101";
        }
        else
        {
            newDate = $"{dateChars[2]}{dateChars[1]}{dateChars[0]}";
        }

        
        
        return newDate;
    }
}
