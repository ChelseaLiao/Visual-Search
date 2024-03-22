using UnityEngine;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using VRception;

/// <summary>
/// ExportToCSV.class
/// 
/// version 1.0
/// date: July 1st, 2020
/// authors: Martin Feick & Niko Kleer
/// </summary>

namespace VRQuestionnaireToolkit
{
    public class ExportToCSV : MonoBehaviour
    {
        public string Delimiter;

        public DataManager dataManager;
        public BlockController blockController;
        public DataLogger dataLogger;

        private GameObject _pageFactory;
        private GameObject _vrQuestionnaireToolkit;
        private string _questionnaireID;

        public UnityEvent QuestionnaireFinishedEvent;

        // Use this for initialization
        void Start()
        {
            _vrQuestionnaireToolkit = GameObject.FindGameObjectWithTag("VRQuestionnaireToolkit");

            if (QuestionnaireFinishedEvent == null) { 
                QuestionnaireFinishedEvent = new UnityEvent();
            }
        }

        public void Save()
        {
            _pageFactory = GameObject.FindGameObjectWithTag("QuestionnaireFactory");
            List<string[]> _csvRows = new List<string[]>();

            long now = System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
            string[] csvTemp;

            // enable all GameObjects (except the first and last page) in order to read the responses
            for (int i = 1; i < _pageFactory.GetComponent<PageFactory>().NumPages - 1; i++) { 
                _pageFactory.GetComponent<PageFactory>().PageList[i].SetActive(true);
            }

            #region CONSTRUCTING RESULTS
            // read participants' responses 
            for (int i = 0; i < _pageFactory.GetComponent<PageFactory>().QuestionList.Count; i++)
            {
                if (_pageFactory.GetComponent<PageFactory>().QuestionList[i] != null)
                {
                    csvTemp = new string[6];
                    csvTemp[0] = now.ToString();
                    csvTemp[1] = blockController.getBlockName();
                    if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Radio>() != null)
                    {
                        _questionnaireID = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Radio>().QuestionnaireId;

                        csvTemp[2] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Radio>().QType;
                        csvTemp[3] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Radio>().QText;
                        csvTemp[4] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Radio>().QId;

                        for (int j = 0;
                            j < _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Radio>()
                                .RadioList.Count;
                            j++)
                        {
                            if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInChildren<Toggle>().isOn)
                            {
                                if (_questionnaireID != "SSQ")
                                {
                                    csvTemp[5] = "" + (j + 1);
                                }
                                else
                                {
                                    csvTemp[5] = "" + j;
                                }
                            }
                        }
                        _csvRows.Add(csvTemp);
                    }
                    else if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<LinearGrid>() != null)
                    {
                        _questionnaireID = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<LinearGrid>().QuestionnaireId;

                        csvTemp[2] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<LinearGrid>().QType;
                        csvTemp[3] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<LinearGrid>().QText;
                        csvTemp[4] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<LinearGrid>().QId;


                        for (int j = 0;
                            j < _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<LinearGrid>()
                                .LinearGridList.Count;
                            j++)
                        {
                            if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInChildren<Toggle>().isOn)
                            {
                                csvTemp[5] = "" + (j + 1);
                            }
                        }

                        _csvRows.Add(csvTemp);
                    }
                    else if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>() != null)
                    {
                        _questionnaireID = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>().QuestionnaireId;
                        csvTemp[2] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>().QType;
                        csvTemp[3] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>().QConditions + "_" + _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>().QText;
                        csvTemp[4] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>().QId;

                        for (int j = 0;
                            j < _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<RadioGrid>()
                                .RadioList.Count;
                            j++)
                        {
                            if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInChildren<Toggle>().isOn)
                            {
                                csvTemp[5] = "" + (j + 1);
                            }
                        }
                        _csvRows.Add(csvTemp);
                    }
                    else if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Checkbox>() != null)
                    {
                        _questionnaireID = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Checkbox>().QuestionnaireId;

                        for (int j = 0;
                            j < _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Checkbox>()
                                .CheckboxList.Count;
                            j++)
                        {
                            csvTemp = new string[6];
                            csvTemp[2] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Checkbox>().QType;
                            csvTemp[3] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Checkbox>().QText + " -" +
                                        _pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInParent<Checkbox>().QOptions[j]; // "xxxQuestionxxx? -xxxOptionxxx"
                            csvTemp[4] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Checkbox>().QId;
                            csvTemp[5] = (_pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInChildren<Toggle>().isOn ? ("" + 1) : ""); // 1 if checked, blank if unchecked
                            _csvRows.Add(csvTemp);
                        }
                    }
                    else if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Slider>() != null)
                    {
                        _questionnaireID = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Slider>().QuestionnaireId;
                        csvTemp[2] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Slider>().QType;
                        csvTemp[3] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Slider>().QText;
                        csvTemp[4] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Slider>().QId;


                        for (int j = 0;
                            j < _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Slider>()
                                .SliderList.Count;
                            j++)
                        {
                            csvTemp[5] = "" + _pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInChildren<UnityEngine.UI.Slider>().value;
                        }
                        _csvRows.Add(csvTemp);
                    }
                    else if (_pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Dropdown>() != null)
                    {
                        _questionnaireID = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Dropdown>().QuestionnaireId;
                        csvTemp[2] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Dropdown>().QType;
                        csvTemp[3] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Dropdown>().QText;
                        csvTemp[4] = _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Dropdown>().QId;


                        for (int j = 0;
                            j < _pageFactory.GetComponent<PageFactory>().QuestionList[i][0].GetComponentInParent<Dropdown>()
                                .DropdownList.Count;
                            j++)
                        {
                            csvTemp[5] = "" + _pageFactory.GetComponent<PageFactory>().QuestionList[i][j].GetComponentInChildren<TMP_Dropdown>().value;
                        }
                        _csvRows.Add(csvTemp);
                    }
                }
            }
            #endregion

            // disable all GameObjects (except the last page) 
            for (int i = 1; i < _pageFactory.GetComponent<PageFactory>().NumPages - 1; i++)
                _pageFactory.GetComponent<PageFactory>().PageList[i].SetActive(false);

            StringBuilder contentOfResult = new StringBuilder();

            foreach (string[] sa in _csvRows)
                contentOfResult.AppendLine(string.Join(Delimiter, sa));

            dataLogger.writeQuestionnaire(contentOfResult);

            QuestionnaireFinishedEvent.Invoke(); //notify 
        }
    }
}

