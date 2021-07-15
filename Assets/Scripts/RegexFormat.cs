using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.IO;
using TMPro;
using System.Text.RegularExpressions;

public class RegexFormat : MonoBehaviour
{
    public Dictionary<int, List<string>> _data = new Dictionary<int, List<string>>();
    public Dictionary<string, List<string>> _stockPrices = new Dictionary<string, List<string>>();
    [SerializeField]
    TMP_Text _inputDisplay;
    [SerializeField]
    GameObject _controller;
    [SerializeField]
    TMP_Text _statusDisplay;
    [SerializeField]
    Toggle _toggleWhole;

    bool isRemovingWhole = false;
    int entry = 0;
    private int _length;

    public void FormatCSVtoQFX(string input)
    {
        //Cut single line into seperate lines
        _inputDisplay.text = "(empty)";
        string[] separatingStrings = { ",,,,", "DEFERRED SALES CHARGE" };
        string[] outputLines = input.Split(separatingStrings, 0);
        _inputDisplay.text = "";


        int i = 0;
        //Find length
        _length = outputLines.Length;

        //Display the text in the input box
        foreach (var word in outputLines)
        {
            
            _inputDisplay.text += $"{word} \n";
            if (i == 0)
            {
                _inputDisplay.text += " DEFERRED SALES CHARGE";
            }
            i++;
            
        }

        //Cut lines into words
        string[] seperatingWords = { ",","aaaaaaa"};
        string[] outputWords = new string[_length];
        int lineCount = 0;

        //Cycle per line
        foreach (var line in outputLines)
        {
           
            outputLines[lineCount] = line;
            string currentLine = outputLines[lineCount];
            string[] currentWord = currentLine.Split(seperatingWords, 0);

            int wordcount = 0;
            //Define all variables
            string date = "";
            string tranID = "";
            string desc = "";
            string quantity = "";
            string symbol = "";
            string price = "";
            string commission = "";
            string amount = "";
            string regFee = "";
            string shortRDM = "";
            string fundRedemption = "";
            string deferredFee = "";

            //Cycle per word
            foreach (var word in currentWord)
            {
                //Exclude first and last line
                if (lineCount > 0 && lineCount < _length - 1)
                {
                    //Sort word into categories based on word count in line
                    switch(wordcount)
                    {
                        case 0:
                            date = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 1:
                            tranID = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 2:
                            desc = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 3:
                            quantity = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 4:
                            symbol = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 5:
                            price = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 6:
                            commission = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 7:
                            amount = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 8:
                            regFee = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 9:
                            shortRDM = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 10:
                            fundRedemption = word.Replace("\r", "").Replace("\n", "");
                            break;

                        case 11:
                            deferredFee = word.Replace("\r", "").Replace("\n", "");
                            break;

                        default:
                            break;
                    }
                }
                

                wordcount++;
            }

            //Create dictionary entry once per line
            if (lineCount != _length-1 && lineCount != 0)
            {
                
                if (isRemovingWhole == false)
                {
                    CreateStockDictionary(new object[] {symbol, price, date });
                    AddDataEntry(new object[] { entry, date, tranID, desc, quantity, symbol, price, commission, amount, regFee, shortRDM, fundRedemption, deferredFee });
                    //print("notON");
                    
                }
                else
                {
                    bool success = int.TryParse(quantity,out int testFloat);
                        if (!success)
                        {
                        CreateStockDictionary(new object[] { symbol, price, date });
                        AddDataEntry(new object[] { entry, date, tranID, desc, quantity, symbol, price, commission, amount, regFee, shortRDM, fundRedemption, deferredFee });
                        //print("fail even" + entry + "  " + quantity);
                        
                    }
                        else
                    {
                        //print("pass even" + entry + "  " + quantity);
                    }

                }
                entry++;

            }


            lineCount++;
        }

       
        //Pass data to next script
        ExportOFX _nextScript = _controller.GetComponent<ExportOFX>();
        _nextScript.ConvertToOFX(_data, _stockPrices);
        
    }

    void AddDataEntry(params object[] entry)
    {
        //Initialize all variables
        int key = (int)entry[0];
        string date = (string)entry[1];
        string tranID = (string)entry[2];
        string desc = (string)entry[3];
        string quantity = (string)entry[4];
        string symbol = (string)entry[5];
        string price = (string)entry[6];
        string commission = (string)entry[7];
        string amount = (string)entry[8];
        string regFee = (string)entry[9];
        string shortRDM = (string)entry[10];
        string fundRedemption = (string)entry[11];
        string deferredFee = (string)entry[12];

        //Build list
        List<string> entryList = new List<string>();
        entryList.Insert(0, date); //0
        entryList.Insert(1, tranID); //1
        entryList.Insert(2, desc); //2
        entryList.Insert(3, quantity); //3
        entryList.Insert(4, symbol); //4
        entryList.Insert(5, price); //5
        entryList.Insert(6, commission); //6
        entryList.Insert(7, amount); //7
        entryList.Insert(8, regFee); //8
        entryList.Insert(9, shortRDM); //9
        entryList.Insert(10, fundRedemption); //10
        entryList.Insert(11, deferredFee); //11

        //Create dictionary
        _data.Add(key, entryList);
    }

    public void UpdateCheckBox()
    {
        isRemovingWhole = _toggleWhole.isOn;
    }


    void CreateStockDictionary(params object[] entry)
    {
        string symbol = (string)entry[0];
        string price = (string)entry[1];
        string dateString = (string)entry[2];
        string[] dateChars;
        string year = "";
        string month = "";
        string day = "";
        if (dateString != "***END OF FILE***")
        {
            dateChars = dateString.Split('/');
            year = dateChars[2];
            month = dateChars[0];
            day = dateChars[1];

            //foreach (string datechar in dateChars)
            //{
            //    date += datechar;
            //}

            if (_stockPrices.ContainsKey(symbol)) //contains key already
            {
                if (int.Parse(year) >= int.Parse(_stockPrices[symbol][1])) //year is equal or greater then previous entry
                {
                    if (int.Parse(month) >= int.Parse(_stockPrices[symbol][2])) //month is equal or greater then previous entry
                    {
                        if (int.Parse(day) >= int.Parse(_stockPrices[symbol][0])) //year is equal or greater then previous entry
                        {
                            //Debug.Log(month + "  " + day + "  /  " + _stockPrices[symbol][0]);
                            List<string> entryList = new List<string>();
                            entryList.Insert(0, price); //0
                            entryList.Insert(1, year); //0
                            entryList.Insert(2, month); //0
                            entryList.Insert(3, day); //0
                            entryList.Insert(4, symbol); //0
                                                         //Create dictionary
                            _stockPrices.Add(symbol, entryList);

                        }
                        else //day is lower then previous entry
                        {
                            //Debug.Log(month + "  " + day + "  /  " + _stockPrices[symbol][0]);
                        }
                    }
                    else //month is lower then previous entry
                    {
                        //Debug.Log(month + "  " + day + "  /  " + _stockPrices[symbol][0]);
                    }
                }
                else //year is lower then previous entry
                {
                    //print(year);
                }
            }
            else //create entry
            {
                //print("create entry");
                List<string> entryList = new List<string>();
                entryList.Insert(0, price); //0
                entryList.Insert(1, year); //0
                entryList.Insert(2, month); //0
                entryList.Insert(3, day); //0
                entryList.Insert(4, symbol); //0
                //Create dictionary
                _stockPrices.Add(symbol, entryList);
            }
        }


        




    }

    void iniDictionaryStockPrices()
    {

    }
}
