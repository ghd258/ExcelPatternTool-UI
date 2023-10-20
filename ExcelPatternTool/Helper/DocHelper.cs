﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExcelPatternTool.Core.Excel.Core;
using ExcelPatternTool.Core.Excel.Models;
using ExcelPatternTool.Core.Excel.Models.Interfaces;
using ExcelPatternTool.Core.Helper;
using ExcelPatternTool.Model;
using ExcelPatternTool.Core.EntityProxy;
using System.Collections;

namespace ExcelPatternTool.Helper
{
    public class DocHelper
    {
        private static string _fileName = "未命名Excel";
        private static string _excelFilesXlsxXls = "Excel 2007文件|*.xlsx|Excel 97-2003文件|*.xls";
        private static readonly string basePath = CommonHelper.AppBasePath;

        public static void SaveTo<T>(IList<T> src, ExportOption exportOption) where T : IExcelEntity
        {


            string path = Path.Combine(basePath, "Data");

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory =path;
            saveFileDialog.Filter = _excelFilesXlsxXls;
            saveFileDialog.FileName = _fileName;
            //saveFileDialog.DefaultExt = "xlsx";
            //xlsx导出太慢了，故改用xls
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;

            // Show save file dialog box
            bool? result = saveFileDialog.ShowDialog();
            //点了保存按钮进入
            if (result == true)
            {
                var filePath = saveFileDialog.FileName;
                var extension = Path.GetExtension(filePath);
                Exporter exporter = new Exporter();
                if (extension.EndsWith("xls"))
                {
                    exporter.DumpXls(filePath);

                }
                else
                {
                    exporter.DumpXlsx(filePath);

                }
                var buff = exporter.ProcessGetBytes(src, exportOption);
                FileStream fs;
                try
                {
                    fs = new FileStream(filePath, FileMode.Create);
                    fs.Write(buff, 0, buff.Length);
                    fs.Close();


                }
                catch (Exception e)
                {
                    MessageBox.Show(filePath + " 此文件正被其他程序占用");
                }

            }
        }

        public static void SaveTo(Type entityType, IList src, ExportOption exportOption)
        {
            string path = Path.Combine(basePath, "Data");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = path;
            saveFileDialog.Filter = _excelFilesXlsxXls;
            saveFileDialog.FileName = _fileName;
            //saveFileDialog.DefaultExt = "xlsx";
            //xlsx导出太慢了，故改用xls
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.AddExtension = true;
            saveFileDialog.RestoreDirectory = true;

            // Show save file dialog box
            bool? result = saveFileDialog.ShowDialog();
            //点了保存按钮进入
            if (result == true)
            {
                var filePath = saveFileDialog.FileName;
                var extension = Path.GetExtension(filePath);
                Exporter exporter = new Exporter();
                if (extension.EndsWith("xls"))
                {
                    exporter.DumpXls(filePath);

                }
                else
                {
                    exporter.DumpXlsx(filePath);

                }
                var buff = exporter.ProcessGetBytes(entityType, src, exportOption);
                FileStream fs;
                try
                {
                    fs = new FileStream(filePath, FileMode.Create);
                    fs.Write(buff, 0, buff.Length);
                    fs.Close();


                }
                catch (Exception e)
                {
                    MessageBox.Show(filePath + " 此文件正被其他程序占用");
                }

            }
        }

        public static IList<T> ImportFrom<T>(ImportOption importOption = null) where T : IExcelEntity
        {

            string path = Path.Combine(basePath, "Data");
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = _excelFilesXlsxXls;
            openFileDialog.FileName = _fileName;
            openFileDialog.AddExtension = true;
            openFileDialog.RestoreDirectory = true;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var filePath = openFileDialog.FileName;
                return ImportFromPath<T>(filePath, importOption);
            }
            else
            {
                return default;
            }
        }

        public static IList<IExcelEntity> ImportFrom(ImportOption importOption = null)
        {

            string path = Path.Combine(basePath, "Data");
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = _excelFilesXlsxXls;
            openFileDialog.FileName = _fileName;
            openFileDialog.AddExtension = true;
            openFileDialog.RestoreDirectory = true;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var filePath = openFileDialog.FileName;
                return ImportFromPath(filePath, importOption);
            }
            else
            {
                return default;
            }
        }

        public static List<T> ImportFromPath<T>(string filePath, ImportOption importOption) where T : IExcelEntity
        {
            if (importOption == null)
            {
                importOption = new ImportOption(EntityProxyContainer.Current.EntityType, 0, 0);
            }
            Importer import = new Importer();
            List<T> output = new List<T>();

            var data1 = new byte[0];
            try
            {
                data1 = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(filePath + " 此文件正被其他程序占用");
                return output;
            }

            try
            {
                if (filePath.EndsWith(".xlsx"))
                {
                    import.LoadXlsx(data1);
                }
                else if (filePath.EndsWith(".xls"))
                {
                    import.LoadXls(data1);
                }
                else
                {
                    MessageBox.Show(filePath + " 文件类型错误");
                    return output;
                }

                output = import.Process<T>(importOption).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(filePath + " 格式错误");
            }

            return output;


        }

        public static List<IExcelEntity> ImportFromPath(string filePath, ImportOption importOption)
        {
            if (importOption == null)
            {
                importOption = new ImportOption(EntityProxyContainer.Current.EntityType, 0, 0);
            }
            Importer import = new Importer();
            List<IExcelEntity> output = new List<IExcelEntity>();

            var data1 = new byte[0];
            try
            {
                data1 = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(filePath + " 此文件正被其他程序占用");
                return output;
            }

            try
            {
                if (filePath.EndsWith(".xlsx"))
                {
                    import.LoadXlsx(data1);
                }
                else if (filePath.EndsWith(".xls"))
                {
                    import.LoadXls(data1);
                }
                else
                {
                    MessageBox.Show(filePath + " 文件类型错误");
                    return output;
                }

                output = import.Process(EntityProxyContainer.Current.EntityType, importOption).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(filePath + " 格式错误");
            }

            return output;


        }

        public static dynamic ImportFromDelegator(Func<Importer, dynamic> action)
        {

            string path = Path.Combine(basePath, "Data");
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = _excelFilesXlsxXls;
            openFileDialog.FileName = _fileName;
            openFileDialog.AddExtension = true;
            openFileDialog.RestoreDirectory = true;
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var filePath = openFileDialog.FileName;
                return ImportFromPathDelegator(filePath, action);
            }
            else
            {
                return default;
            }
        }

        public static dynamic ImportFromPathDelegator(string filePath, Func<Importer, dynamic> action)
        {
            Importer import = new Importer();

            var data1 = new byte[0];
            try
            {
                data1 = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(filePath + " 此文件正被其他程序占用");
                return default;
            }

            try
            {
                if (filePath.EndsWith(".xlsx"))
                {
                    import.LoadXlsx(data1);
                }
                else if (filePath.EndsWith(".xls"))
                {
                    import.LoadXls(data1);
                }
                else
                {
                    MessageBox.Show(filePath + " 文件类型错误");
                    return default;
                }

                return action?.Invoke(import);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var msg = $"{filePath} 格式错误\n 错误信息：{e.Message}";
                MessageBox.Show(msg);
            }

            return default;
        }


    }
}
