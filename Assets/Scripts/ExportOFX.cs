using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class ExportOFX : MonoBehaviour
{
    [SerializeField]
    GameObject _controller;
    string date = "";

    [SerializeField]
    TMP_Text _brokerText;
    [SerializeField]
    TMP_Text _accText;
    [SerializeField]
    TMP_InputField _accField;

    [SerializeField]
    TMP_Text _fileOutputName;

    [SerializeField]
    TMP_Text _inputDisplay;


    int StockCount = 0;
    int StockOrderCount = 0;

    private void Start()
    {
        //Find current date
        date = System.DateTime.Now.ToString("yyyyMMdd");
    }

    public void ConvertToOFX(Dictionary<int, List<string>> input, Dictionary<string, List<string>> stockPrices)
    {
        //reset counter
        StockCount = 0;
        StockOrderCount = 0;
        //Find original path
        OpenFile openScript = _controller.GetComponent<OpenFile>();
        string path = openScript.Path;
        print("ORIGINAL PATH:  " + path);
        string[] orPathArr = path.Split('\\');
        string newPath = "";

        //Put new path in same folder as original path

        int i = 0;
        foreach (var word in orPathArr)
        {
            if (i != orPathArr.Length - 1)
            {
                newPath += orPathArr[i] + "/";
            }
            else //end of the file path
            {
                if (StaticData.isUsingFileName)
                {
                    string[] _newFileArr = orPathArr[i].Split('.');
                    StaticData.CurrentAccountID = _newFileArr[0];
                    print("UsingACCID: " + StaticData.CurrentAccountID);
                }
                orPathArr[orPathArr.Length - 1] = $"{StaticData.OutputName}.ofx";
                newPath += orPathArr[i];
            }


            i++;
        }
        ////For if no file name is set
        //if (newPath == "")
        //{
        //    newPath = 
        //}

        //Overwrite file if exists already
        if (File.Exists(newPath))
        {
            File.Delete(newPath);
        }
        print("Output Location: " + newPath);





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
        writer.WriteLine("<INVSTMTMSGSRSV1>");
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
        writer.WriteLine($"<BROKERID>{StaticData.CurrentBrokerID}</BROKERID>");
        writer.WriteLine($"<ACCTID>{StaticData.CurrentAccountID}</ACCTID>");
        writer.WriteLine("</INVACCTFROM>");
        writer.WriteLine("<INVTRANLIST>");

        #endregion
        #region Transactions

        i = 0;
        string startDate = "";
        foreach (var value in input.Values)
        {
            startDate = ParseDate(value[0]);
            i++;
        }

        writer.WriteLine($"<DTSTART>{startDate}120000</DTSTART>");
        writer.WriteLine($"<DTEND>{date}120000</DTEND>");
        foreach (var value in input.Values)
        {
            if (value[2] != "")
            {
                #region WRITE STOCK
                writer.WriteLine("<REINVEST>");
                writer.WriteLine("<INVTRAN>");
                writer.WriteLine($"<FITID>{value[1]}</FITID>");

                string dateNoTime;
                if (value[0] != "***END OF FILE***")
                {
                    dateNoTime = ParseDate(value[0]);
                }
                else
                {
                    dateNoTime = "20210101";
                }

                writer.WriteLine($"<DTTRADE>{dateNoTime}120000</DTTRADE>");
                writer.WriteLine($"<DTSETTLE>{dateNoTime}120000</DTSETTLE>");

                writer.WriteLine($"<MEMO>{value[2]}</MEMO>");
                writer.WriteLine("</INVTRAN>");

                writer.WriteLine("<SECID>");
                writer.WriteLine($"<UNIQUEID>{value[4]}001</UNIQUEID>");
                writer.WriteLine("<UNIQUEIDTYPE>CUSIP</UNIQUEIDTYPE>");
                writer.WriteLine("</SECID>");


                writer.WriteLine("<INCOMETYPE>DIV</INCOMETYPE>");
                // writer.WriteLine($"<COMMISSION>{value[6]}</COMMISSION>");
                writer.WriteLine($"<TOTAL>{value[7]}</TOTAL>");
                writer.WriteLine("<SUBACCTSEC>CASH</SUBACCTSEC>");
                //writer.WriteLine("<SUBACCTFUND>CASH</SUBACCTFUND>");
                //writer.WriteLine("<BUYTYPE>BUY</BUYTYPE>");
                writer.WriteLine($"<UNITS>{value[3]}</UNITS>");
                writer.WriteLine($"<UNITPRICE>{value[5]}</UNITPRICE>");

                writer.WriteLine("</REINVEST>");
                #endregion

                StockOrderCount += 1;
                print("STOCK BUY COMPLETED: " + StockOrderCount);
            }
        }


        #endregion
        writer.WriteLine("</INVTRANLIST>");
        writer.WriteLine("</INVSTMTRS>");
        #endregion

        writer.WriteLine("</INVSTMTTRNRS>");
        writer.WriteLine("</INVSTMTMSGSRSV1>");
        #endregion

        #region WRITE STOCK INFO
        writer.WriteLine("<SECLISTMSGSRSV1>");
        writer.WriteLine("<SECLIST>");
        foreach (var stocks in stockPrices.Values)
        {
            writer.WriteLine("<STOCKINFO>");
            writer.WriteLine("<SECINFO>");
            writer.WriteLine("<SECID>");
            writer.WriteLine($"<UNIQUEID>{stocks[4]}001</UNIQUEID>");
            writer.WriteLine("<UNIQUEIDTYPE>CUSIP</UNIQUEIDTYPE>");
            writer.WriteLine("</SECID>");
            writer.WriteLine($"<SECNAME>{stocks[4]}</SECNAME>");
            writer.WriteLine($"<TICKER>{stocks[4]}</TICKER>");
            writer.WriteLine($"<UNITPRICE>{stocks[0]}</UNITPRICE>");
            writer.WriteLine($"<DTASOF>{stocks[1]}{stocks[2]}{stocks[3]}120000</DTASOF>");
            writer.WriteLine("</SECINFO>");
            writer.WriteLine("</STOCKINFO>");

            StockCount += 1;
            print("STOCK INFO COMPLETED: " + StockCount);
        }




        writer.WriteLine("</SECLIST>");
        writer.WriteLine("</SECLISTMSGSRSV1>");
        #endregion
        writer.WriteLine("</OFX>");
        writer.Close();

        #endregion

        string inputText = _inputDisplay.text;
        _inputDisplay.text = $"Stock Buys: {StockOrderCount} \nUnique Stocks: {StockCount} \n\n\n" + inputText;


        Debug.Log("END");
    }


    string ParseDate(string dateInput)
    {
        string newDate = "";
        if (dateInput != "***END OF FILE***")
        {

            //Format date into date+hour
            string[] dateChars = dateInput.Split('/');
            if (dateChars[2] == null || dateChars[1] == null || dateChars[0] == null)
            {
                newDate = "20210101120000";
            }
            else
            {
                newDate = $"{dateChars[2]}{dateChars[0]}{dateChars[1]}";
            }

            //string newDate = $"{dateChars[2]}{dateChars[1]}{dateChars[0]}120000";
        }

        return newDate;
    }
}
