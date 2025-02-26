﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;
using ThreadState = System.Threading.ThreadState;
using System.ComponentModel;
using ComboBox = System.Windows.Controls.ComboBox;

namespace DspFindSeed
{
    public partial class MainWindow
    {
        private void FetchPlanetTypeCondition(ComboBox planetType,int index,SearchCondition condition)
        {
            var select = planetType.SelectedIndex;
            if (select < 0)
                select = 0;
            condition.planetNames[index] = select;
            if (select > 0)
            {
                if(!condition.planetNameCounts.ContainsKey(select))
                    condition.planetNameCounts.Add(select,1);
                else
                    condition.planetNameCounts[select]++;
            }
        }
        private SearchCondition FetchCondition ()
        {
            var condition = new SearchCondition ();
            condition.planetCount1     = int.Parse (planetCount1.Text);
            condition.planetCount2     = int.Parse (planetCount2.Text);
            condition.planetCount3     = int.Parse (planetCount3.Text);
            condition.gasSpeed         = float.Parse (MaxGasSpeed.Text);
            condition.dysonLumino      = float.Parse (dysonLumino.Text);
            condition.distanceToBirth  = float.Parse (distanceToBirth.Text);
            condition.isInDsp          = IsInDsp.IsChecked ?? false;
            condition.isInDsp2         = IsInDsp2.IsChecked ?? false;
            condition.resourceCount[0] = int.Parse (resource0.Text);
            condition.resourceCount[1] = int.Parse (resource1.Text);
            condition.resourceCount[2] = int.Parse (resource2.Text);
            condition.resourceCount[3] = int.Parse (resource3.Text);
            condition.resourceCount[4] = int.Parse (resource4.Text);
            condition.resourceCount[5] = int.Parse (resource5.Text);
            var select = StarType.SelectedIndex;
            if (select < 0)
                select = 0;
            condition.starType         = (enumStarType)select;
            FetchPlanetTypeCondition(PlanetType1, 0, condition);
            FetchPlanetTypeCondition(PlanetType2, 1, condition);
            FetchPlanetTypeCondition(PlanetType3, 2, condition);
            FetchPlanetTypeCondition(PlanetType4, 3, condition);
            FetchPlanetTypeCondition(PlanetType5, 4, condition);
            FetchPlanetTypeCondition(PlanetType6, 5, condition);
            
            var boolResource6 = resource6.IsChecked ?? false;
            condition.resourceCount[6]  = boolResource6 ? 1 : 0;
            condition.resourceCount[7]  = int.Parse (resource7.Text);
            condition.resourceCount[8]  = int.Parse (resource8.Text);
            condition.resourceCount[9]  = int.Parse (resource9.Text);
            condition.resourceCount[10] = int.Parse (resource10.Text);
            condition.resourceCount[11] = int.Parse (resource11.Text);
            condition.resourceCount[12] = int.Parse (resource12.Text);
            condition.resourceCount[13] = int.Parse (resource13.Text);

            condition.hasWater          = resource1000.IsChecked ?? false;
            condition.hasAcid           = resource1116.IsChecked ?? false;
            condition.starCount         = int.Parse (starCount.Text);
            condition.IsLogResource     = IsLogResource.IsChecked ?? false;
            return condition;
        }

        private void SetCondition (SearchCondition condition)
        {
            planetCount1.Text      = condition.planetCount1.ToString ();
            planetCount2.Text      = condition.planetCount2.ToString ();
            planetCount3.Text      = condition.planetCount3.ToString ();
            MaxGasSpeed.Text       = condition.gasSpeed.ToString (CultureInfo.InvariantCulture);
            dysonLumino.Text       = condition.dysonLumino.ToString (CultureInfo.InvariantCulture);
            distanceToBirth.Text   = condition.distanceToBirth.ToString (CultureInfo.InvariantCulture);
            StarType.SelectedIndex = condition.starType.GetHashCode ();
            
            PlanetType1.SelectedIndex = condition.planetNames[0].GetHashCode ();
            PlanetType2.SelectedIndex = condition.planetNames[1].GetHashCode ();
            PlanetType3.SelectedIndex = condition.planetNames[2].GetHashCode ();
            PlanetType4.SelectedIndex = condition.planetNames[3].GetHashCode ();
            PlanetType5.SelectedIndex = condition.planetNames[4].GetHashCode ();
            PlanetType6.SelectedIndex = condition.planetNames[5].GetHashCode ();
            
            IsInDsp.IsChecked  = condition.isInDsp;
            IsInDsp2.IsChecked = condition.isInDsp2;

            resource0.Text  = condition.resourceCount[0].ToString ();
            resource1.Text  = condition.resourceCount[1].ToString ();
            resource2.Text  = condition.resourceCount[2].ToString ();
            resource3.Text  = condition.resourceCount[3].ToString ();
            resource4.Text  = condition.resourceCount[4].ToString ();
            resource5.Text  = condition.resourceCount[5].ToString ();
            resource7.Text  = condition.resourceCount[7].ToString ();
            resource8.Text  = condition.resourceCount[8].ToString ();
            resource9.Text  = condition.resourceCount[9].ToString ();
            resource10.Text = condition.resourceCount[10].ToString ();
            resource11.Text = condition.resourceCount[11].ToString ();
            resource12.Text = condition.resourceCount[12].ToString ();
            resource13.Text = condition.resourceCount[13].ToString ();

            resource6.IsChecked     = condition.resourceCount[6] > 0;
            resource1000.IsChecked  = condition.hasWater;
            resource1116.IsChecked  = condition.hasAcid;
            IsLogResource.IsChecked = condition.IsLogResource;

            starCount.Text = condition.starCount.ToString ();
        }

        
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var msg = "确认退出吗？";
            if (curThread != null)
            {
                msg += "\n正在搜索ID，搜索到 ：" + curId + ";\n 已命中种子数量：" + curSeeds + ";最后命中的是：" + lastSeedId + "是否终止搜索并退出程序？";
            }
            else
            {
                msg += "\n当前搜索已结束或者尚未搜索，可以安全退出";
            }
            if (MessageBox.Show(msg, "退出询问", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                if (curThread != null)
                    curThread.Abort();
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void ComboBox_SelectionChanged_Necessary(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            curSelectIndex = necessaryCondition.SelectedIndex;
            if (curSelectIndex < 0)
                return;
            SetCondition (searchNecessaryConditions[curSelectIndex]);
            curSelectLog = false;
        }

        private void ComboBox_SelectionChanged_Log(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            curSelectIndex = LogCondition.SelectedIndex;
            if (curSelectIndex < 0)
                return;
            SetCondition (searchLogConditions[curSelectIndex]);
            curSelectLog = true;
        }

        private void Button_Click_AddNecessary(object sender, System.Windows.RoutedEventArgs e)
        {
            searchNecessaryConditions.Add (FetchCondition ());
            necessaryCondition.Items.Add ("条件" + searchNecessaryConditions.Count);
            curSelectIndex = searchNecessaryConditions.Count - 1;
            necessaryCondition.Text = (string)necessaryCondition.Items[curSelectIndex];
        }

        private void Button_Click_AddLog(object sender, System.Windows.RoutedEventArgs e)
        {
            searchLogConditions.Add (FetchCondition ());
            LogCondition.Items.Add ("条件" + searchLogConditions.Count);
            curSelectIndex = searchLogConditions.Count - 1;
            LogCondition.Text = (string)LogCondition.Items[curSelectIndex];
        }

        private void Button_Click_Del(object sender, System.Windows.RoutedEventArgs e)
        {
            if (curSelectIndex < 0)
                return;
            if (curSelectLog)
            {
                searchLogConditions.RemoveAt(curSelectIndex);
                RefreshConditionUi ();
            }
            else
            {
                searchNecessaryConditions.RemoveAt(curSelectIndex);
                RefreshConditionUi ();
            }
        }

        private void RefreshConditionUi ()
        {
            var cacheIndex = curSelectIndex;
            necessaryCondition.Items.Clear();
            LogCondition.Items.Clear();
            for (int i = 0; i < searchNecessaryConditions.Count; i++)
            {
                necessaryCondition.Items.Add ("条件" + i);
            }
            for (int i = 0; i < searchLogConditions.Count; i++)
            {
                LogCondition.Items.Add ("条件" + i);
            }
            
            if (curSelectLog)
            {
                if(searchLogConditions.Count <= cacheIndex)
                {
                    cacheIndex = searchLogConditions.Count - 1;
                }
                curSelectIndex = cacheIndex;
                if (curSelectIndex < 0)
                    return;
                LogCondition.SelectedIndex = curSelectIndex;
                LogCondition.Text          = (string)LogCondition.Items[curSelectIndex];
                SetCondition (searchLogConditions[curSelectIndex]);
            }
            else
            {
                if (searchNecessaryConditions.Count <= cacheIndex)
                {
                    cacheIndex = searchNecessaryConditions.Count - 1;
                }
                curSelectIndex = cacheIndex;
                if (curSelectIndex < 0)
                    return;
                necessaryCondition.SelectedIndex = curSelectIndex;
                necessaryCondition.Text          = (string)necessaryCondition.Items[curSelectIndex];
                SetCondition (searchNecessaryConditions[curSelectIndex]);
            }
               
        }

        private void Button_Click_ImportFile(object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title       = "请选择文件夹";
            dialog.Filter      = "json|*.json";
            dialog.InitialDirectory = saveConditionPath;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string text = File.ReadAllText(dialog.FileName);
                jsonCondition = JsonConvert.DeserializeObject<JsonCondition>(text);
                searchNecessaryConditions = jsonCondition.searchNecessaryConditions;
                searchLogConditions       = jsonCondition.searchLogConditions;
                curSelectIndex            = 0;
                curSelectLog              = false;
                RefreshConditionUi ();
            }
        }

        private void Button_Click_ExportFile(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            dialog.SelectedPath = saveConditionPath;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                saveConditionPath = dialog.SelectedPath;
                SaveConditionPath.Text = saveConditionPath;
                jsonCondition.searchNecessaryConditions = searchNecessaryConditions;
                jsonCondition.searchLogConditions = searchLogConditions;
                //string text = JsonMapper.ToJson(jsonCondition);
                string text = JsonConvert.SerializeObject(jsonCondition);
                fileName = FileName.Text;
                System.IO.File.WriteAllText(saveConditionPath + "\\conditon_" + fileName + ".json", text, Encoding.UTF8);
            }
        }

        private void Button_Click_ImportSeedFile (object sender, System.Windows.RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect      = false;
            dialog.Title            = "请选择文件夹";
            dialog.Filter           = "csv|*.csv";
            dialog.InitialDirectory = saveConditionPath;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var path    = Path.GetDirectoryName (dialog.FileName);
               
                if (!CsvUtil.OpenCSV (Path.GetDirectoryName (dialog.FileName) + "\\"+ Path.GetFileName (dialog.FileName), out CustomSeedIdS,out CustomSeedStarCounts))
                {
                    if (MessageBox.Show ("该文件没有找到正确的种子ID", "重新输入",MessageBoxButtons.OKCancel ) == System.Windows.Forms.DialogResult.OK)
                    {
                        Button_Click_ImportSeedFile (sender, e);
                    }
                }
            }
        }

        private void Button_Click_ConfigSave(object sender, System.Windows.RoutedEventArgs e)
        {
            searchConfig.times = int.Parse (searchTimes.Text);
            searchConfig.onceCount = int.Parse (searchOnceCount.Text);
            searchConfig.magCount = int.Parse(MagCount.Text);
            searchConfig.bluePlanetCount = int.Parse (BluePlanetCount.Text);
            searchConfig.oPlanetCount = int.Parse (OPlanetCount.Text);
            searchConfig.curMinSearchStarSelectIndex = SearchMinStarCount.SelectedIndex;
            searchConfig.curMaxSearchSelectIndex = SearchMaxStarCount.SelectedIndex;
            string text = JsonConvert.SerializeObject(searchConfig);
            System.IO.File.WriteAllText(saveConditionPath + "\\config.json", text, Encoding.UTF8);
            MessageBox.Show("保存成功，下次打开会继承当前搜索的全局的配置，包括单次搜索数量、搜索次数、恒星数区间、磁石、蓝巨、0型数量", "成功", MessageBoxButtons.OKCancel);
        }

        private void Button_Click_Stop (object sender, System.Windows.RoutedEventArgs e)
        {
            if (curThread == null || curThread.ThreadState == ThreadState.Aborted || curThread.ThreadState == ThreadState.Stopped)
                return;
            var curTime = (DateTime.Now - startTime).TotalSeconds;
            if(MessageBox.Show ("正在搜索ID，搜索到 ：" + curId + "；已用时：" + curTime + ";\n 已命中种子数量：" + curSeeds + ";最后命中的是：" + lastSeedId + "是否终止搜索？" , "终止搜索",MessageBoxButtons.OKCancel ) == System.Windows.Forms.DialogResult.OK)
            {
                curThread.Abort();
                curThread = null;
            }
          
        }

        void normalSearch ()
        {
            curSeeds        = 0;
            lastSeedId      = 0;
            fileName        = FileName.Text;
            magCount        = int.Parse(MagCount.Text);
            bluePlanetCount = int.Parse (BluePlanetCount.Text);
            oPlanetCount = int.Parse (OPlanetCount.Text);
            if (curThread != null)
                curThread.Abort();
            curThread = new Thread(StartSearchSeed);
            curThread.Start();
        }
        private void Button_Click_Start(object sender, System.Windows.RoutedEventArgs e)
        {
            var minStarCountItem = SearchMinStarCount.SelectionBoxItem;
            var maxStarCountItem = SearchMaxStarCount.SelectionBoxItem;
            var minStarCount = minStarCountItem != null && minStarCountItem.GetType() != typeof(string) ? (int) minStarCountItem : -1;
            var maxStarCount = maxStarCountItem != null && maxStarCountItem.GetType() != typeof(string) ? (int) maxStarCountItem : -1;
            if (minStarCount <= 0 &&  maxStarCount <=0)
            {
                MessageBox.Show("星区数量至少选一个值，代表搜指定数量的；选两个代表搜区间内的", "失败", MessageBoxButtons.OKCancel);
                return;
            }
            curMinSearchStarCount = 0;
            curMaxSearchStarCount = 0;
            if (minStarCount <= 0)
            {
                curMinSearchStarCount = maxStarCount;
                curMaxSearchStarCount = curMinSearchStarCount;
            }
            else if (maxStarCount <= 0)
            {
                curMinSearchStarCount = minStarCount;
                curMaxSearchStarCount = curMinSearchStarCount;
            }
            else if (minStarCount == maxStarCount)
            {
                curMinSearchStarCount = minStarCount;
                curMaxSearchStarCount = curMinSearchStarCount;
            }
            else if (maxStarCount > minStarCount)
            {
                curMinSearchStarCount = minStarCount;
                curMaxSearchStarCount = maxStarCount;
            }
            else if (maxStarCount < minStarCount)
            {
                curMinSearchStarCount = maxStarCount;
                curMaxSearchStarCount = minStarCount;
            }
            switch (StartType.SelectedIndex)
            {
                case 0:
                    startId   = int.Parse (seedID.Text);
                    onceCount = int.Parse (searchOnceCount.Text);
                    times     = int.Parse (searchTimes.Text);
                    logInit   = false;
                    var total = onceCount * times;
                    if (total > 1000000)
                    {
                        if(MessageBox.Show ("开始ID：" + startId + "，每次搜索：" + onceCount + "个，一共搜索：" + times + "次，是否开始搜索？（总搜索次数大于100万会弹出，以防误操作）" , "开始搜索",MessageBoxButtons.OKCancel ) == System.Windows.Forms.DialogResult.OK)
                        {
                            normalSearch ();
                        }
                    }
                    else
                    {
                        normalSearch ();
                    }
                   
                    break;
                case 1 :
                    startId  = int.Parse(seedID.Text);
                    fileName = FileName.Text + "_single";
                    logInit  = false;
                    times    = int.Parse(searchTimes.Text);
                    if (curThread != null)
                        curThread.Abort();
                    curThread = new Thread(SingleSearch);
                    curThread.Start();
                    break;
                case 2 :
                    onceCount       = int.Parse(searchOnceCount.Text);
                    times           = int.Parse(searchTimes.Text);
                    logInit         = false;
                    curSeeds        = 0;
                    lastSeedId      = 0;
                    fileName        = FileName.Text;
                    magCount        = int.Parse(MagCount.Text);
                    bluePlanetCount = int.Parse (BluePlanetCount.Text);
                    oPlanetCount    = int.Parse (OPlanetCount.Text);
                    if (curThread != null)
                        curThread.Abort();
                    curThread = new Thread(SearchCustomId);
                    curThread.Start();
                    break;
            }
           
        }
    }
}