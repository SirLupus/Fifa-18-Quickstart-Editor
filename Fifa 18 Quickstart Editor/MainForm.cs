/*
 * Created by SharpDevelop.
 * User: Wolfgang
 * Date: 11.10.2017
 * Time: 14:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Microsoft.Win32;
using IniParser;
using IniParser.Model;

namespace Fifa_18_Quickstart_Editor
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();			
			InitComboBox();
			BackupFiles();
			CreateLabel();
		}

		void CreateLabel() {
		var pos = this.PointToScreen(label1.Location);
        pos = pictureBox1.PointToClient(pos);
        label1.Parent = pictureBox1;
        label1.Location = pos;
        label1.BackColor = Color.Transparent;
 		}
		
		void InitComboBox()
		{
			var languages = new string[2];
			languages[0] = "ger,de";
			languages[1] = "eng,us";

			DataSet myDataSet = new DataSet();

			// --- Preparation
			DataTable lTable = new DataTable("Lang");
			DataColumn lName = new DataColumn("Language", typeof(string));
			lTable.Columns.Add(lName);

			for (int i = 0; i < languages.Length; i++) {
				DataRow lLang = lTable.NewRow();
				lLang["Language"] = languages[i];
				lTable.Rows.Add(lLang);
			}
			myDataSet.Tables.Add(lTable);

			comboBox1.DataSource = myDataSet.Tables["Lang"].DefaultView;
			comboBox1.DisplayMember = "Language";
			comboBox1.BindingContext = this.BindingContext;
		}
		void BackupFiles()
		{
			String filePath = getFifaInstallPath();
			try {
				File.Copy(filePath + "Data\\locale.ini", filePath + "Data\\locale.bak", false);
				File.Copy(filePath + "FIFASetup\\config.ini", filePath + "FIFASetup\\config.bak", false);
			}
			 // Catch exception if the file was already copied.
        catch (IOException copyError) {
				// Do nothing, files are already backed up
			}
		}
		
		public String getFifaInstallPath()
		{
			//HKEY_LOCAL_MACHINE\SOFTWARE\EA Sports\FIFA 18
			String installDir = "unknown";

			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\EA Sports\\FIFA 18");  
			if (key != null) {  
				installDir = key.GetValue("Install Dir").ToString();
				key.Close();  
			}  
			return installDir;
		}
		
		void Button1Click(object sender, EventArgs e)
		{

		}
		void PictureBox1Click(object sender, EventArgs e)
		{
	
		}
		void Button2Click(object sender, EventArgs e)
		{	// Savebutton
			
			//Save Launcher Settings
			
			bool skipLauncher = checkBox1.Checked;
			bool skipLanguage = checkBox2.Checked;
			String language = comboBox1.GetItemText(comboBox1.SelectedItem);
						
			String path = getFifaInstallPath();
			path += "FIFASetup\\config.ini";
			
			String launcherText = "LAUNCH_EXE = fifa18.exe" + System.Environment.NewLine + "SETTING_FOLDER = 'FIFA 18'" + System.Environment.NewLine;
			
			if (skipLauncher) {
				launcherText += "AUTO_LAUNCH = 1" + System.Environment.NewLine;
			} 
			File.WriteAllText(path, launcherText);
			
			// Save Locale Settings
		
			path = getFifaInstallPath() + "Data\\locale.ini";
						
			FileIniDataParser fileIniData = new FileIniDataParser();
			fileIniData.Parser.Configuration.CommentString = "//";
			IniData parsedData = fileIniData.ReadFile(path);			
			
			String languageIni = parsedData["LOCALE"]["DEFAULT_LANGUAGE"];
			String lanuageSelect = parsedData["LOCALE"]["USE_LANGUAGE_SELECT"];
						
			if (skipLanguage) {
				parsedData["LOCALE"]["USE_LANGUAGE_SELECT"] = "0";
				parsedData["LOCALE"]["DEFAULT_LANGUAGE"] = language;
			} else {
				parsedData["LOCALE"]["USE_LANGUAGE_SELECT"] = "1";
				parsedData["LOCALE"]["DEFAULT_LANGUAGE"] = language;
			}

			fileIniData.WriteFile(path, parsedData);

			ResourceManager resources = new ResourceManager("Fifa_18_Quickstart_Editor.MainForm", Assembly.GetExecutingAssembly());
			Bitmap bitmap = (Bitmap) resources.GetObject("success_kid"); //image without extension
			pictureBox1.Image = bitmap;
			label1.Visible = true;
			//MessageBox.Show("Success. You may now start Fifa ;)");
		}


	}
}
