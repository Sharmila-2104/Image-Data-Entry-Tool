using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;

using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

namespace Ireland
{
    public partial class Form1 : Form
    {
        bool nxt;
      //  private double zoomFactor = 1.0;
        private System.Drawing.Image originalImage;
        FileInfo current_image;

       // DataTable dt = new DataTable();
        List<string> list = new List<string>();
        int img_count;

        int ccount = 0;
        string path;
        int temp;
        int Focus;
        int LCyPos = 0;

        private Point mouseDownLocation;
        private bool isDragging = false;
        string file_path;
        string xmlFilePath;
        private static Form msgBox;
        private List<TextBox> currentRowTextBoxes = new List<TextBox>();
        private string[] Surname;
        private string[] Givename;
        private string[] Residence; 
        private AutoCompleteStringCollection GNautoComplete;
        private AutoCompleteStringCollection SNautoComplete;
        private AutoCompleteStringCollection RautoComplete;

        //string imgtype = "";
        //string[] spsimg;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true; // Ensure the form captures key events
            //this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            Givename = File.ReadAllLines("C:\\Image\\SPS_IDBMD_Givename.ini");
            Surname = File.ReadAllLines("C:\\Image\\SPS_IDBMD_Surname.ini");
            Residence = File.ReadAllLines("C:\\Image\\SPS_IDBMD_Residence.ini");

            GNautoComplete = new AutoCompleteStringCollection();
            GNautoComplete.AddRange(Givename);

            SNautoComplete = new AutoCompleteStringCollection();
            SNautoComplete.AddRange(Surname);

            RautoComplete = new AutoCompleteStringCollection();
            RautoComplete.AddRange(Residence);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //sps image support 
            // original path

            var dir = Directory.GetCurrentDirectory();
            var current_dir = Directory.GetDirectories(dir);
            path = current_dir[0].ToString();
            if (current_dir.Length > 0)
            {
                // path = current_dir[0].ToString();
            }
            else
            {
                MessageBox.Show("No directories found in the current directory.");
                return;
            }

            //path = @"D:\Sharmila\project Requirement files\ID_BMD\New folder\ID_BMD_SPS\op\microfilm_02410_05\microfilm_02410_05";
            string[] imagePath = Directory.GetFiles(path, "*.sps");

            foreach (var img in imagePath)
            {
                string fileName = Path.GetFileNameWithoutExtension(img);
                list.Add(fileName);
            }

            // Sort the list naturally
            list.Sort(new NaturalSortComparer<string>());

            // Add sorted filenames to ComboBox
            foreach (var fileName in list)
            {
                comboBox1.Items.Add(fileName);
            }

            img_count = list.Count();
            current_image = new FileInfo(list[ccount]);
            lbl_img.Text = current_image.ToString();

            // Decrypt and load the first image
            string imagePath3 = Path.Combine(path, current_image + ".sps");
            using (Stream decryptedStream = DecryptFilestream(imagePath3))
            {
                originalImage = System.Drawing.Image.FromStream(decryptedStream);
            }

            pictureBox1.Image = originalImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            panel1.Controls.Add(pictureBox1);

            // Select the first item in the ComboBox
            nxt = false;
            comboBox1.SelectedIndex = 0;
            nxt = true;
            txt_count.Text = (temp + 1) + "/" + img_count;

            btn_zoom_in.Click += btn_zoom_in_Click;
            btn_ZoomOut.Click += btn_ZoomOut_Click;
            trackBar1.Scroll += trackBar1_Scroll;
            ApplyZoom();

            if (ccount == 0)
            {
                btn_Previous.Enabled = false;
            }

            Focus = 1;
            FocusFirstTextBox();

            string[] column_name = {"SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Maiden Name", "Gender", "Age", "Birth day",
                           "Birth month", "Birth year", "Baptism day", "Baptism month", "Baptism year", "Death day", "Death month", "Death year", "Burial day",
                           "BurialMonth", "Burial Year", "Residence", "Father Given Name", "Father Surname", "Mother Given Name", "MotherSurname",
                           "MotherMaidenName", "Groom Given Name", "Groom Surname", "Groom Residence", "GroomFatherGivenName", "GroomFatherSurname",
                           "GroomMotherGivenName", "GroomMotherSurname", "GroomMotherMaidenName", "BrideGivenName", "BrideSurname", "BrideMaidenName",
                           "Bride Residence", "BrideFatherGiven_Name", "BrideFatherSurname", "BrideMotherGiven_Name", "BrideMotherSurname",
                           "BrideMotherMaidenName", "Marriageday", "Marriagemonth", "Marriageyear","Census year","Confirmation day","Confirmation month","Confirmation year", "UID","NOTES"};


            int folderIdIndex = Array.IndexOf(column_name, "Folder ID");
            if (folderIdIndex != -1 && folderIdIndex < panel3.Controls.Count)
            {
                var control = panel3.Controls[folderIdIndex];
                if (control is TextBox folderIdTextBox)
                {
                    string folderName = new DirectoryInfo(path).Name;
                    folderIdTextBox.Text = folderName;
                }
            }

            int imageIdIndex = Array.IndexOf(column_name, "Image ID");
            if (imageIdIndex != -1 && imageIdIndex < panel3.Controls.Count)
            {
                var control = panel3.Controls[imageIdIndex];
                if (control is TextBox imageIdTextBox)
                {
                    imageIdTextBox.Text = Path.GetFileNameWithoutExtension(current_image.Name);
                }
            }
        }
        public class NaturalSortComparer<T> : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == null || y == null)
                    return 0;

                int len1 = x.Length, len2 = y.Length;
                int marker1 = 0, marker2 = 0;

                while (marker1 < len1 && marker2 < len2)
                {
                    char ch1 = x[marker1];
                    char ch2 = y[marker2];

                    // Some buffers we can build up characters in for each chunk
                    char[] space1 = new char[len1];
                    int loc1 = 0;
                    char[] space2 = new char[len2];
                    int loc2 = 0;

                    // Collect a chunk of characters for each string
                    do
                    {
                        space1[loc1++] = ch1;
                        marker1++;

                        if (marker1 < len1)
                        {
                            ch1 = x[marker1];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                    do
                    {
                        space2[loc2++] = ch2;
                        marker2++;

                        if (marker2 < len2)
                        {
                            ch2 = y[marker2];
                        }
                        else
                        {
                            break;
                        }
                    } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                    string str1 = new string(space1);
                    string str2 = new string(space2);

                    int result;

                    if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                    {
                        int thisNumericChunk = int.Parse(str1);
                        int thatNumericChunk = int.Parse(str2);
                        result = thisNumericChunk.CompareTo(thatNumericChunk);
                    }
                    else
                    {
                        result = str1.CompareTo(str2);
                    }

                    if (result != 0)
                    {
                        return result;
                    }
                }

                return len1 - len2;
            }
        }

        //private void AddPicturesToInnerPanel()
        //{
        //    // path = @"D:\Sharmila\ID_BMD\New folder\microfilm_02410_05";
        //    path = @"D:\Sharmila\project Requirement files\ID_BMD\New folder\ID_BMD\op\microfilm_02410_05\microfilm_02410_05";
        //    string[] imagePaths = Directory.GetFiles(path, "*.sps");

        //    // Loop through the image paths and create PictureBox controls
        //    foreach (string path in imagePaths)
        //    {
        //        PictureBox pictureBox = new PictureBox();
        //        // pictureBox.Image = Image.FromFile(path);

        //        pictureBox.Image = System.Drawing.Image.FromFile(path);
        //        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

        //        // Add the PictureBox to the inner panel
        //        Panel panel5 = new Panel();
        //        //panel5.Width = 2000;
        //        //panel5.Height = 700;
        //        panel5.AutoScroll = true;
        //        panel5.Controls.Add(pictureBox);
        //        //  zoomFactor -= 0.1;
        //        ApplyZoom();
        //    }
        //}

        private const float MinZoomFactor = 0.1f;
        private const float MaxZoomFactor = 5.0f;
        private float zoomFactor = 1.0f;
        private void btn_ZoomOut_Click(object sender, EventArgs e)
        {
            // zoomFactor += 0.1;
            zoomFactor = Math.Min(zoomFactor + 0.1f, MaxZoomFactor);
            ApplyZoom();
        }
        private void btn_zoom_in_Click(object sender, EventArgs e)
        {
            zoomFactor = Math.Max(zoomFactor - 0.1f, MinZoomFactor);
            //zoomFactor -= 0.1;
            ApplyZoom();
        }
        private void ApplyZoom()
        {
            // Ensure the original image is not null
            if (originalImage == null)
            {
                MessageBox.Show("Original image is not loaded.");
                return;
            }

            // Calculate new dimensions
            int newWidth = (int)(originalImage.Width * zoomFactor);
            int newHeight = (int)(originalImage.Height * zoomFactor);

            // Ensure new dimensions are valid
            if (newWidth <= 0 || newHeight <= 0)
            {
                MessageBox.Show("Invalid zoom factor resulting in non-positive dimensions.");
                return;
            }

            // Create a new bitmap with the scaled dimensions
            Bitmap bitmap = new Bitmap(newWidth, newHeight);

            // Use Graphics to draw the resized image
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight));
            }

            // Set the new image with the applied zoom
            pictureBox1.Image = bitmap;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // Calculate the new location
                pictureBox1.Left += e.X - mouseDownLocation.X;
                pictureBox1.Top += e.Y - mouseDownLocation.Y;
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                mouseDownLocation = e.Location;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = false;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            float brightness = (trackBar1.Value - 5) / 5.0f;

            // Adjust the brightness of the current image and update the PictureBox
            if (originalImage != null)
            {
                Bitmap adjustedImage = AdjustBrightness((Bitmap)originalImage, brightness);
                pictureBox1.Image = adjustedImage;
            }
        }
        private Bitmap AdjustBrightness(Bitmap original, float brightness)
        {
            // Create a blank bitmap the same size as the original
            Bitmap adjustedImage = new Bitmap(original.Width, original.Height);

            // Create a graphics object from the blank bitmap
            using (Graphics g = Graphics.FromImage(adjustedImage))
            {
                // Create a ColorMatrix to adjust brightness
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {brightness, brightness, brightness, 0, 1}
                    });

                // Create an ImageAttributes object and set its color matrix
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);

                // Draw the original image onto the blank bitmap using the color matrix
                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                            0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }
            return adjustedImage;
        }

        public Bitmap changeintensity(System.Drawing.Image img, System.Drawing.Image curimg, int intensity)
        {
            if (intensity >= 0 && intensity <= 10)
            {
                trackBar1.Value = intensity;
            }
            else
            {
                MessageBox.Show("You have reached maximum or minimum Intensity level");
            }
            Size newSize = new Size((int)(curimg.Width * zoomFactor), (int)(curimg.Height * zoomFactor));
            Bitmap bmp = new Bitmap(img, newSize);
            float brightness = (trackBar1.Value - 5) / 5.0f;
            Bitmap adjustedImage = AdjustBrightness(bmp, brightness);
            if (chk_Negative.Checked)
            {
                adjustedImage = ConvertToNegativeLockbit(adjustedImage);
            }
            return adjustedImage;
        }

        private Bitmap ConvertToNegativeLockbit(Bitmap original)
        {
            // Lock the bitmap's bits
            Rectangle rect = new Rectangle(0, 0, original.Width, original.Height);
            BitmapData bmpData = original.LockBits(rect, ImageLockMode.ReadWrite, original.PixelFormat);

            // Get the address of the first line
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap
            int bytes = Math.Abs(bmpData.Stride) * original.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Invert the colors
            for (int i = 0; i < rgbValues.Length; i += 4)
            {
                rgbValues[i] = (byte)(255 - rgbValues[i]);     // Blue
                rgbValues[i + 1] = (byte)(255 - rgbValues[i + 1]); // Green
                rgbValues[i + 2] = (byte)(255 - rgbValues[i + 2]); // Red
            }

            // Copy the RGB values back to the bitmap
            Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits
            original.UnlockBits(bmpData);

            return original;
        }

        private void chk_Negative_CheckedChanged(object sender, EventArgs e)
        {
            Size newSize = new Size((int)(pictureBox1.Image.Width), (int)(pictureBox1.Image.Height));
            Bitmap bmp = new Bitmap(pictureBox1.Image, newSize);
            Bitmap negativeImage = ConvertToNegativeLockbit(bmp);
            pictureBox1.Image = negativeImage;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Control control in panel3.Controls.OfType<Control>().ToList())
            {
                if (control is TextBox || control is ComboBox)
                {
                    panel3.Controls.Remove(control);
                }
            }

            txt_ins.Clear();
            txt_del.Clear();
            chk_Negative.Checked = false;
            Focus = 1;
            string selectedName = comboBox1.SelectedItem as string + ".sps";
            current_image = new FileInfo(selectedName);
            int selectedIndex = comboBox1.SelectedIndex;
            txt_count.Text = (selectedIndex + 1) + "/" + img_count;

            if (selectedIndex == 0)
            {
                btn_Previous.Enabled = false;
            }

            if (selectedName != null)
            {
                ccount = selectedIndex;
                var image_path = Path.Combine(path, selectedName);

                using (Stream decryptedStream = DecryptFilestream(image_path))
                {
                    System.Drawing.Image originalImage = System.Drawing.Image.FromStream(decryptedStream);
                    pictureBox1.Image = originalImage;
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                #region LoadXMLData
                string outputDirectory = Path.Combine(path, "XML-Output");
                string currentImageName = current_image.ToString().Split('.').First();
                string xmlpath = Path.Combine(outputDirectory, $"{currentImageName}.xml");
                string selectedFormType = "YourSelectedFormTypeHere";
                if (File.Exists(xmlpath))
                {
                    XDocument doc = XDocument.Load(xmlpath);
                    int numRows = doc.Descendants("Row").Count();
                    DataTable dataTable = CreateDataTable();
                    dataTable = PopulateDataTable(doc, dataTable);
                    CreateTextBoxesAndLabels(numRows, selectedFormType);
                    foreach (DataRow row in dataTable.Rows)
                    {
                        LoadDataFromXml(current_image.ToString(), row["SNo"].ToString());
                    }
                }
                #endregion

                if (selectedIndex != 0)
                {
                    btn_Previous.Enabled = true;
                }
            }

            if (Focus == 1)
            {
                Focus = Focus + 1;
            }

            //foreach (Control control in panel3.Controls.OfType<Control>().ToList())
            //{
            //    if (control is TextBox || control is ComboBox)
            //    {
            //        panel3.Controls.Remove(control);
            //    }
            //}
            //// panel3.Controls.Clear();
            //txt_ins.Clear();
            //txt_del.Clear();
            //chk_Negative.Checked = false;
            //Focus = 1;
            //string selectedName = comboBox1.SelectedItem as string + ".sps";
            //current_image = new FileInfo(selectedName);
            //int selectedIndex = comboBox1.SelectedIndex;
            //txt_count.Text = (selectedIndex + 1) + "/" + img_count;
            //if (selectedIndex == 0)
            //{
            //    btn_Previous.Enabled = false;
            //}
            //if (selectedName != null)
            //{
            //    ccount = selectedIndex;
            //    var image_path = path + "//" + selectedName;
            //    System.Drawing.Image originalImage = System.Drawing.Image.FromFile(image_path);
            //    // originalImage = Image.FromFile(image_path);
            //    pictureBox1.Image = originalImage;
            //    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            //    //Loadxmldata
            //    #region LoadXMLData
            //    string outputDirectory = Path.Combine(path, "XML-Output");
            //    string currentImageName = current_image.ToString().Split('.').First();
            //    string xmlpath = Path.Combine(outputDirectory, $"{currentImageName}.xml");
            //    string selectedFormType = "YourSelectedFormTypeHere";
            //    if (File.Exists(xmlpath))
            //    {
            //        XDocument doc = XDocument.Load(xmlpath);
            //        int numRows = doc.Descendants("Row").Count();
            //        DataTable dataTable = CreateDataTable();
            //        dataTable = PopulateDataTable(doc, dataTable);
            //        CreateTextBoxesAndLabels(numRows, selectedFormType);
            //        foreach (DataRow row in dataTable.Rows)
            //        {
            //            LoadDataFromXml(current_image.ToString(), row["SNo"].ToString());
            //        }
            //    }
            //    #endregion
            //    // ShowMarriageFields();
            //    if (selectedIndex != 0)
            //    {
            //        btn_Previous.Enabled = true;
            //    }
            //}
            //if (Focus == 1)
            //{
            //    //FocusFirstTextBox();
            //    Focus = Focus + 1;
            //}
        }
        // Method to load data from XML to controls

        public Stream DecryptFilestream(string encryptedImageFile)
        {
            byte[] ImageBytes;
            ImageBytes = File.ReadAllBytes(encryptedImageFile);

            for (int i = 0; i < ImageBytes.Length; i++)
            {
                ImageBytes[i] = (byte)(ImageBytes[i] ^ 2132135641564654);
            }

            return new MemoryStream(ImageBytes);
        }

        private void LoadDataFromXml(string imageFileName, string sNo)
        {
            string outputDirectory = Path.Combine(path, "XML-Output");
            // Ensure the output directory exists
            if (!Directory.Exists(outputDirectory))
            {
                MessageBox.Show("Output directory does not exist.");
                return;
            }

            // Define the path for the XML file based on imageFileName
            string currentImageName = imageFileName.ToString().Split('.').First();
            string xmlFilePath = Path.Combine(outputDirectory, $"{currentImageName}.xml");

            if (!File.Exists(xmlFilePath))
            {
                return;
            }

            try
            {
                // Load the XML document
                XDocument doc = XDocument.Load(xmlFilePath);

                // Find the Row element by SNo
                XElement row = doc.Root.Elements("Row")
                                .FirstOrDefault(r => r.Attribute("SNo")?.Value == sNo);

                if (row == null)
                {
                    return;
                }

                // Define all column names (matching your control sequence)
                string[] columnNames = {
               "SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname",
               "Maiden Name", "Gender", "Age", "Birth day", "Birth month", "Birth year", "Baptism day",
               "Baptism month", "Baptism year", "Death day", "Death month", "Death year", "Burial day",
               "Burial month", "Burial Year", "Residence", "Father Given Name", "Father Surname",
               "Mother Given Name", "Mother Surname", "Mother Maiden Name", "Groom Given Name", "Groom Surname",
               "Groom Residence", "Groom Father Given Name", "Groom Father Surname", "Groom Mother Given Name",
               "Groom Mother Surname", "Groom Mother Maiden Name", "Bride Given Name", "Bride Surname",
               "Bride Maiden Name", "Bride Residence", "Bride Father Given_Name", "Bride Father Surname",
               "Bride Mother Given_Name", "Bride Mother Surname", "Bride Mother Maiden Name", "Marriage day",
               "Marriage month", "Marriage year","Census year","Confirmation day","Confirmation month",
               "Confirmation year", "UID","NOTES"};

                string[] columns = {
                "SNo", "FolderID", "ImageID", "FormType", "LineNumber", "Page", "GivenName", "Surname",
                "MaidenName", "Gender", "Age", "BirthDay", "BirthMonth", "BirthYear", "BaptismDay",
                "BaptismMonth", "BaptismYear", "DeathDay", "DeathMonth", "DeathYear", "BurialDay",
                "BurialMonth", "BurialYear", "Residence", "FatherGivenName", "FatherSurname",
                "MotherGivenName", "MotherSurname", "MotherMaidenName", "GroomGivenName", "GroomSurname",
                "GroomResidence", "GroomFatherGivenName", "GroomFatherSurname", "GroomMotherGivenName",
                "GroomMotherSurname", "GroomMotherMaidenName", "BrideGivenName", "BrideSurname",
                "BrideMaidenName", "BrideResidence", "BrideFatherGivenName", "BrideFatherSurname",
                "BrideMotherGivenName", "BrideMotherSurname", "BrideMotherMaidenName", "MarriageDay",
                "MarriageMonth", "MarriageYear", "CensusYear", "ConfirmationDay", "ConfirmationMonth",
                "ConfirmationYear", "UID","NOTES"};

                // Get controls in the current row
                var controls = panel3.Controls.Cast<Control>().ToArray();
                int endvalue = Convert.ToInt32(sNo) * columns.Length;
                int startvalue = endvalue - columns.Length;
                int j = startvalue - startvalue;
                // Load data from the XML attributes into controls
                for (int i = startvalue; i < endvalue; i++)
                {
                    string columnName = columns[j];
                    string value = row.Attribute(columnName)?.Value ?? "";

                    // Set the value to the corresponding control (e.g., TextBox or ComboBox)
                    if (controls.ElementAtOrDefault(i) is TextBox textBox)
                    {
                        textBox.Text = value;
                    }
                    else if (controls.ElementAtOrDefault(i) is ComboBox comboBox)
                    {
                        comboBox.SelectedItem = value;
                    }
                    j++;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private DataTable CreateDataTable()
        {
            DataTable dataTable = new DataTable();

            string[] columns = {
                "SNo", "FolderID", "ImageID", "FormType", "LineNumber", "Page", "GivenName", "Surname",
                "MaidenName", "Gender", "Age", "BirthDay", "BirthMonth", "BirthYear", "BaptismDay",
                "BaptismMonth", "BaptismYear", "DeathDay", "DeathMonth", "DeathYear", "BurialDay",
                "BurialMonth", "BurialYear", "Residence", "FatherGivenName", "FatherSurname",
                "MotherGivenName", "MotherSurname", "MotherMaidenName", "GroomGivenName", "GroomSurname",
                "GroomResidence", "GroomFatherGivenName", "GroomFatherSurname", "GroomMotherGivenName",
                "GroomMotherSurname", "GroomMotherMaidenName", "BrideGivenName", "BrideSurname",
                "BrideMaidenName", "BrideResidence", "BrideFatherGivenName", "BrideFatherSurname",
                "BrideMotherGivenName", "BrideMotherSurname", "BrideMotherMaidenName", "MarriageDay",
                "MarriageMonth", "MarriageYear", "CensusYear", "ConfirmationDay", "ConfirmationMonth",
                "ConfirmationYear", "UID","NOTES", "CurrentDate"
            };

            foreach (string column in columns)
            {
                dataTable.Columns.Add(column);
            }

            return dataTable;
        }
        private DataTable PopulateDataTable(XDocument xmlDoc, DataTable dataTable)
        {
            foreach (XElement imElement in xmlDoc.Root.Elements("Row"))
            {
                DataRow row = dataTable.NewRow();
                //row["ImageNumber"] = imElement.Attribute("ImageNumber")?.Value;
                //row["ImageType"] = imElement.Attribute("ImageType")?.Value;

                // Iterate through each <r> node
                foreach (XAttribute attribute in imElement.Attributes())
                {
                    row[attribute.Name.LocalName] = attribute.Value;
                }
                //row["Status"] = imElement.Attribute("Status")?.Value;
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        private void btn_Previous_Click(object sender, EventArgs e)
        {
            ccount = ccount - 1;
            if (ccount >= 0)
            {
                //panel3.Controls.Clear();
                foreach (Control control in panel3.Controls.OfType<Control>().ToList())
                {
                    if (control is TextBox || control is ComboBox)
                    {
                        panel3.Controls.Remove(control);
                    }
                }
                txt_ins.Clear();
                txt_del.Clear();
                chk_Negative.Checked = false;

                nxt = false;
                Focus = 1;
                temp = ccount;
                txt_count.Text = (temp + 1) + "/" + img_count;

                // Ensure the file has the correct extension
                string currentFileName = list[temp] + ".sps";  // Adjust the extension as needed
                current_image = new FileInfo(currentFileName);

                comboBox1.SelectedIndex = temp;

                // Combine path and file name correctly
                var image_path = Path.Combine(path, current_image.ToString());

                try
                {
                    // If the file requires decryption, use the decryption method here.
                    using (Stream decryptedStream = DecryptFilestream(image_path))
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromStream(decryptedStream);
                        pictureBox1.Image = originalImage;
                        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    }
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("File not found: " + image_path);
                    return; // Exit early if the file is not found
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                    return; // Exit early if there's an error loading the image
                }

                btn_Next.Enabled = true;

                if (Focus == 1)
                {
                    // FocusFirstTextBox();
                    Focus = Focus + 1;
                }
            }
            else
            {
                MessageBox.Show("You are on the first image");
                nxt = false;
                Focus = 1;
                temp = 0;
                txt_count.Text = (temp + 1) + "/" + img_count;
                current_image = new FileInfo(list[temp] + ".sps");  // Ensure extension
                comboBox1.SelectedIndex = temp;
                var image_path = Path.Combine(path, current_image.ToString());

                //// Load the image
                //try
                //{
                //    System.Drawing.Image originalImage = System.Drawing.Image.FromFile(image_path);
                //    pictureBox1.Image = originalImage;
                //    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                //}
                //catch (FileNotFoundException)
                //{
                //    MessageBox.Show("File not found: " + image_path);
                //}
                // Load and display the first image again
                try
                {
                    using (Stream decryptedStream = DecryptFilestream(image_path))
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromStream(decryptedStream);
                        pictureBox1.Image = originalImage;
                        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                    }
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("File not found: " + image_path);
                }

                btn_Previous.Enabled = false;

                if (Focus == 1)
                {
                    // FocusFirstTextBox();
                    Focus = Focus + 1;
                }
            }

        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            // Ensure we only move to the next image if we're not at the end
            if (ccount + 1 < img_count)
            {
                foreach (Control control in panel3.Controls.OfType<Control>().ToList())
                {
                    if(control is TextBox || control is ComboBox)
                    {
                        panel3.Controls.Remove(control);
                    }
                }
                // Update UI components
                // panel3.Controls.Clear();
               

                txt_ins.Clear();
                txt_del.Clear();
                chk_Negative.Checked = false;
                ccount++;  // Increment the image counter

                nxt = false;
                Focus = 1;
                temp = ccount;  // Assign the incremented value to temp
                txt_count.Text = (temp + 1) + "/" + img_count;

                // Ensure the file has the correct extension
                string currentFileName = list[temp] + ".sps";  // Adjust the extension as needed
                current_image = new FileInfo(currentFileName);

                // Combine path and file name correctly
                var image_path = Path.Combine(path, current_image.ToString());

                //// Load the image
                //try
                //{
                //    originalImage = System.Drawing.Image.FromFile(image_path);
                //    pictureBox1.Image = originalImage;
                //    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                //}
                //catch (FileNotFoundException)
                //{
                //    MessageBox.Show("File not found: " + image_path);
                //    return; // Exit early if the file is not found
                //}

                // Load and decrypt the image
                try
                {
                    using (Stream decryptedStream = DecryptFilestream(image_path))
                    {
                        originalImage = System.Drawing.Image.FromStream(decryptedStream);
                    }

                    pictureBox1.Image = originalImage;
                    pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("File not found: " + image_path);
                    return; // Exit early if the file is not found
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                    return; // Exit early if there's an error loading the image
                }

                // Enable the Previous button, as we're no longer on the first image
                btn_Previous.Enabled = true;

                nxt = true;  // Set flag to true indicating the next image is loaded

                // Update the ComboBox selection after the increment
                comboBox1.SelectedIndex = temp;

                if (Focus == 1)
                {
                    // If focus needs to be moved to the first text box (uncomment when needed)
                    // FocusFirstTextBox();
                    Focus++;
                }
            }
            else
            {
                // If we are at the last image, disable the Next button
                MessageBox.Show("You are on the last image");
                btn_Next.Enabled = false;
            }
            //ccount = ccount + 1;
            //if (ccount != img_count)
            //{
            //    panel3.Controls.Clear();
            //    txt_ins.Clear();
            //    txt_del.Clear();
            //    nxt = false;
            //    Focus = 1;
            //    temp = ccount;
            //    txt_count.Text = (temp + 1) + "/" + img_count;

            //    // Ensure the file has the correct extension
            //    string currentFileName = list[temp] + ".sps";  // Adjust the extension as needed
            //    current_image = new FileInfo(currentFileName);

            //    comboBox1.SelectedIndex = temp;

            //    // Combine path and file name correctly
            //    var image_path = Path.Combine(path, current_image.ToString());

            //    // Load the image
            //    try
            //    {
            //        originalImage = System.Drawing.Image.FromFile(image_path);
            //        pictureBox1.Image = originalImage;
            //        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            //    }
            //    catch (FileNotFoundException)
            //    {
            //        MessageBox.Show("File not found: " + image_path);
            //    }

            //    btn_Previous.Enabled = true;
            //    nxt = true;
            //    if (Focus == 1)
            //    {
            //        // FocusFirstTextBox();
            //        Focus = Focus + 1;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("You are on the last image");
            //    btn_Next.Enabled = false;
            //}
        }

        //panel1
        private void AdjustHorizontalScroll(int increment)
        {
            int newValue = panel1.HorizontalScroll.Value + increment;

            // Ensure the new value is within the valid range
            if (newValue > panel1.HorizontalScroll.Maximum)
            {
                newValue = panel1.HorizontalScroll.Maximum;
            }
            else if (newValue < panel1.HorizontalScroll.Minimum)
            {
                newValue = panel1.HorizontalScroll.Minimum;
            }

            panel1.HorizontalScroll.Value = newValue;
            panel1.PerformLayout();
        }
        private void AdjustVerticalScroll(int increment)
        {
            int newValue = panel1.VerticalScroll.Value + increment;

            // Ensure the new value is within the valid range
            if (newValue > panel1.VerticalScroll.Maximum)
            {
                newValue = panel1.VerticalScroll.Maximum;
            }
            else if (newValue < panel1.VerticalScroll.Minimum)
            {
                newValue = panel1.VerticalScroll.Minimum;
            }

            panel1.VerticalScroll.Value = newValue;
            panel1.PerformLayout();
        }

        //panel3
        //private void panelAdjustHorizontalScroll(int increment)
        //{
        //    int newValue = panel3.HorizontalScroll.Value + increment;

        //    // Ensure the new value is within the valid range
        //    if (newValue > panel3.HorizontalScroll.Maximum)
        //    {
        //        newValue = panel3.HorizontalScroll.Maximum;
        //    }
        //    else if (newValue < panel3.HorizontalScroll.Minimum)
        //    {
        //        newValue = panel3.HorizontalScroll.Minimum;
        //    }

        //    panel3.HorizontalScroll.Value = newValue;
        //    panel3.PerformLayout();
        //}
        //private void panelAdjustVerticalScroll(int increment)
        //{
        //    int newValue = panel3.VerticalScroll.Value + increment;

        //    // Ensure the new value is within the valid range
        //    if (newValue > panel3.VerticalScroll.Maximum)
        //    {
        //        newValue = panel3.VerticalScroll.Maximum;
        //    }
        //    else if (newValue < panel3.VerticalScroll.Minimum)
        //    {
        //        newValue = panel3.VerticalScroll.Minimum;
        //    }

        //    panel3.VerticalScroll.Value = newValue;
        //    panel3.PerformLayout();
        //}

        // Method to move the PictureBox horizontally
        private void MovePictureBoxHorizontally(int offset)
        {
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is PictureBox pictureBox)
                {
                    // Ensure the PictureBox stays within panel bounds
                    int newLeft = pictureBox.Left + offset;
                    if (newLeft >= 0 && newLeft + pictureBox.Width <= panel1.Width)
                    {
                        pictureBox.Left = newLeft;
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        AdjustHorizontalScroll(10); // Adjust the increment value as needed
                        break;

                    case Keys.Left:
                        AdjustHorizontalScroll(-10); // Adjust the decrement value as needed
                        break;

                    case Keys.Down:
                        AdjustVerticalScroll(10); // Adjust the increment value as needed
                        break;

                    case Keys.Up:
                        AdjustVerticalScroll(-10); // Adjust the decrement value as needed
                        break;
                    case Keys.Oemplus:
                        zoomFactor = Math.Min(zoomFactor + 0.1f, MaxZoomFactor);
                        ApplyZoom();
                        break;

                    case Keys.OemMinus:
                        zoomFactor = Math.Max(zoomFactor - 0.1f, MinZoomFactor);
                        ApplyZoom();
                        break;
                    case Keys.D:
                        HandleCtrlD();
                        break;
                    case Keys.N:
                        btn_Next.PerformClick(); ;
                        break;
                    case Keys.P:
                        btn_Previous.PerformClick(); ;
                        break;
                    //case Keys.W:
                    //    pictureBox1.Image = changeintensity(originalImage, pictureBox1.Image, trackBar1.Value + 1);
                    //    break;

                    //case Keys.Q:
                    //    pictureBox1.Image = changeintensity(originalImage, pictureBox1.Image, trackBar1.Value - 1);
                    //    break;


                }
            }
            else
            {
                // Handling PictureBox movement without Ctrl key
                if (e.KeyCode == Keys.Right)
                {
                    MovePictureBoxHorizontally(90); // Move right by 10 pixels
                }
                else if (e.KeyCode == Keys.Left)
                {
                    MovePictureBoxHorizontally(-90); // Move left by 10 pixels
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    // Simulate button click
                    btn_Insert_Click(sender, e);
                }
            }
        }

        private void Shortcut_Click(object sender, EventArgs e)
        {
            string message = @"

                ctrl + P => previous

                ctrl + N => Next
              
                ctrl + D => Same as above 
        
                ctrl + (+) => Image Zoom IN
       
                ctrl + (-) => Image Zoom Out

                ctrl + C => Copy text box 

                shift + tab => previous tab

                tab = > new text box

                ctrl + right arrow => Brightness 

                ctrl + up arrow  => image move Up

                ctrl + down arrow  => image move Down

                shift + B or B  => Formtype change Birth_Baptism
   
             ";
            int width = 400;
            int height = 400;
            string title = "Short keys";
            ShowCustomMessageBox(message, width, height, title);

        }
        private void ShowCustomMessageBox(string message, int width, int height, string title)
        {
            Form popupForm = new Form
            {
                Size = new System.Drawing.Size(width, height),
                FormBorderStyle = FormBorderStyle.FixedToolWindow,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen
            };

            Panel panel = new Panel
            {
                Size = new System.Drawing.Size(width - 20, height - 20),
                AutoScroll = true
            };

            Label label = new Label
            {
                AutoSize = true,
                Text = message,
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(label);
            popupForm.Controls.Add(panel);

            Button closeButton = new Button
            {
                DialogResult = DialogResult.OK,
                Text = "Close",
                Dock = DockStyle.Bottom
            };

            closeButton.Click += (sender, e) => { popupForm.Close(); };

            popupForm.Controls.Add(closeButton);
            popupForm.AcceptButton = closeButton;

            popupForm.ShowDialog();
        }
        private void TextBox_Click(object sender, EventArgs e)
        {
            // Find the TextBox that currently has focus within panel3
            TextBox focusedTextBox = panel3.Controls.OfType<TextBox>().FirstOrDefault(t => t.Focused);
            if (focusedTextBox != null)
            {
                // Set textBox2.Text to the text of the focused TextBox
                // textBox2.Text = focusedTextBox.Text;

                // Invalidate pictureBox1 (this usually means redraw it)
                pictureBox1.Invalidate();
            }
            else
            {
                // Handle case where no TextBox has focus
                //textBox2.Text = "";
            }
        }
        private void FocusFirstTextBox()
        {
            // Retrieve all TextBox controls within panel3, ordered by TabIndex
            var textboxes = panel3.Controls.OfType<TextBox>().OrderBy(tb => tb.TabIndex).ToList();

            // Check if there are any TextBox controls in the list
            if (textboxes.Any())
            {
                // Set focus to the first TextBox in the list
                textboxes[0].Focus();
            }
        }

        private int nextSerialNumber = 1;
        private string currentImageId = string.Empty; // To track the current image ID
        private List<Control> allTextBoxesAndLabels = new List<Control>();
        private Dictionary<string, string[]> formTypeFields = new Dictionary<string, string[]>()
        {
            { "Marriage", new string[] { "SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Groom Given Name", "Groom Surname",
                                         "Groom Residence", "GroomFatherGivenName", "GroomFatherSurname", "GroomMotherGivenName", "GroomMotherSurname",
                                         "GroomMotherMaidenName", "BrideGivenName", "BrideSurname", "BrideMaidenName", "Bride Residence",
                                         "BrideFatherGiven_Name", "BrideFatherSurname", "BrideMotherGiven_Name", "BrideMotherSurname",
                                         "BrideMotherMaidenName", "Marriageday", "Marriagemonth", "Marriageyear", "UID","NOTES"}},
            { "Birth_Baptism", new string[] {"SNo", "Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Gender",
                                              "Birth day", "Birth month", "Birth year", "Baptism day", "Baptism month", "Baptism year",
                                              "Father Given Name", "Father Surname", "Mother Given Name", "MotherSurname", "MotherMaidenName", "UID","NOTES" }},
            { "Death_Burial", new string[] { "SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Maiden Name",
                                             "Gender", "Age", "Birth day", "Birth month", "Birth year", "Death day", "Death month", "Death year",
                                             "Burial day", "BurialMonth", "Burial Year", "Residence", "UID" ,"NOTES"}},
            { "Census", new string[] { "SNo", "Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Gender", 
                                         "Age", "Birth day", "Birth month", "Birth year", "Residence", "Census year", "UID", "NOTES"}},
            { "Confirmation", new string[] { "SNo", "Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Gender",
                                            "Age", "Birth day", "Birth month", "Birth year", "Residence", "Confirmation day", "Confirmation month", "Confirmation year", "UID", "NOTES" }},
            {  "Title",new string[] { "SNo", "Folder ID", "Image ID", "Form Type","NOTES" } },
            {  "Other",new string[] { "SNo", "Folder ID", "Image ID", "Form Type","NOTES" } },
            {  "Blank",new string[] { "SNo", "Folder ID", "Image ID", "Form Type","NOTES" } },
            {  "Illegible",new string[] { "SNo", "Folder ID", "Image ID", "Form Type","NOTES" } },
            {  "Duplicate",new string[] { "SNo", "Folder ID", "Image ID", "Form Type","NOTES" } },
        };

        private void btn_Insert_Click(object sender, EventArgs e)
        {

           
            if (int.TryParse(txt_ins.Text, out int numRows) && numRows > 0)
            {
                // Determine the starting row number based on existing controls
                int startingRowNumber = panel3.Controls.OfType<TextBox>()
                    .Where(tb => tb.Location.X == 0)
                    .Select(tb => int.TryParse(tb.Text, out int number) ? number : 0)
                    .DefaultIfEmpty(0)
                    .Max() + 1;
                nextSerialNumber = startingRowNumber;
                // Assuming 'selectedFormType' is the value of the currently selected form type
                // string selectedFormType = (panel3.Controls.OfType<ComboBox>().FirstOrDefault()?.SelectedItem ?? ).ToString();
                string selectedFormType = "YourSelectedFormTypeHere";
                // Call CreateTextBoxesAndLabels with the number of new rows and the selected form type
                CreateTextBoxesAndLabels(numRows, selectedFormType);

                txt_ins.Clear(); // Clear input field after insertion
            }
        }

        private Dictionary<int, Tuple<TextBox, TextBox>> surnamePairs = new Dictionary<int, Tuple<TextBox, TextBox>>();

        private int CreateTextBoxesAndLabels(int numRowsToAdd, string selectedFormType)
        {
            // Initialize ToolTip
            // ToolTip toolTip = new ToolTip();

            System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
            // Set up the delays for the ToolTip.
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1000;
            toolTip.ReshowDelay = 50;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip.ShowAlways = true;

            // Define the mapping between column names and data types
            Dictionary<string, string> columnDataTypes = new Dictionary<string, string>
            {
                { "Folder ID", "Numeric Type" },
                { "Image ID", "Numeric Type" },
                { "Form Type", "Alphabetic Type" },
                { "Line Number", "Numeric Type" },
                { "Page", "Numeric Type" },
                { "Given Name", "Alphabetic Type" },
                { "Surname", "Alphabetic Type" },
                { "Maiden Name", "Alphabetic Type" },
                { "Gender", "Alphabetic Type" },
                { "Age", "Numeric Type" },
                { "Birth day", "Numeric Type" },
                { "Birth month", "Numeric Type" },
                { "Birth year", "Numeric Type" },
                { "Baptism day", "Numeric Type" },
                { "Baptism month", "Numeric Type" },
                { "Baptism year", "Numeric Type" },
                { "Death day", "Numeric Type" },
                { "Death month", "Numeric Type" },
                { "Death year", "Numeric Type" },
                { "Burial day", "Numeric Type" },
                { "Burial month", "Numeric Type" },
                { "Burial year", "Numeric Type" },
                { "Residence", "Alphabetic Type" },
                { "Father Given Name", "Alphabetic Type" },
                { "Father Surname", "Alphabetic Type" },
                { "Mother Given Name", "Alphabetic Type" },
                { "MotherSurname", "Alphabetic Type" },
                { "MotherMaidenName", "Alphabetic Type" },
                { "Groom Given Name", "Alphabetic Type" },
                { "Groom Surname", "Alphabetic Type" },
                { "Groom Residence", "Alphabetic Type" },
                { "GroomFatherGivenName", "Alphabetic Type" },
                { "GroomFatherSurname", "Alphabetic Type" },
                { "GroomMotherGivenName", "Alphabetic Type" },
                { "GroomMotherSurname", "Alphabetic Type" },
                { "GroomMotherMaidenName", "Alphabetic Type" },
                { "BrideGivenName", "Alphabetic Type" },
                { "BrideSurname", "Alphabetic Type" },
                { "BrideMaidenName", "Alphabetic Type" },
                { "Bride Residence", "Alphabetic Type" },
                { "BrideFatherGiven_Name", "Alphabetic Type" },
                { "BrideFatherSurname", "Alphabetic Type" },
                { "BrideMotherGiven_Name", "Alphabetic Type" },
                { "BrideMotherSurname", "Alphabetic Type" },
                { "BrideMotherMaidenName", "Alphabetic Type" },
                { "Marriageday", "Numeric Type" },
                { "Marriagemonth", "Numeric Type" },
                { "Marriageyear", "Numeric Type" },
                { "Census year", "Numeric Type" },
                { "Confirmation day", "Numeric Type" },
                { "Confirmation month", "Numeric Type" },
                { "Confirmation year", "Numeric Type" },
                { "UID", "Numeric Type" },
                { "NOTES", "Alphabetic Type" }

            };

            string foldername = new DirectoryInfo(path).Name;

            int textBoxCount = 0;
            string newImageId = Path.GetFileNameWithoutExtension(current_image.Name);   

            if (currentImageId != newImageId)
            {
                currentImageId = newImageId;
                nextSerialNumber = 1;
            }

            int labelWidth = 175; // Adjust width as needed 89 //180
            int textBoxWidth = 175; // Adjust width as needed 89 //180
            int rowHeight = 40; // Adjust row height //40//30
            int spacing = 3; // Adjust spacing between rows
            int yPosStart = panel3.Controls.OfType<TextBox>()
                                 .Select(tb => tb.Bottom)
                                 .DefaultIfEmpty(0)
                                 .Max();

            // Define all column names
            string[] column_name = {"SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Maiden Name", "Gender", "Age", "Birth day",
                           "Birth month", "Birth year", "Baptism day", "Baptism month", "Baptism year", "Death day", "Death month", "Death year", "Burial day",
                           "BurialMonth", "Burial Year", "Residence", "Father Given Name", "Father Surname", "Mother Given Name", "MotherSurname",
                           "MotherMaidenName", "Groom Given Name", "Groom Surname", "Groom Residence", "GroomFatherGivenName", "GroomFatherSurname",
                           "GroomMotherGivenName", "GroomMotherSurname", "GroomMotherMaidenName", "BrideGivenName", "BrideSurname", "BrideMaidenName",
                           "Bride Residence", "BrideFatherGiven_Name", "BrideFatherSurname", "BrideMotherGiven_Name", "BrideMotherSurname",
                           "BrideMotherMaidenName", "Marriageday", "Marriagemonth", "Marriageyear","Census year","Confirmation day","Confirmation month","Confirmation year", "UID","NOTES"};

            int yPos = 0;
            // Create labels only if this is the first insertion
            if (panel5.Controls.OfType<Label>().Count() == 0)
            {
                for (int i = 0; i < column_name.Length; i++)
                {
                    Label label = new Label();
                    label.Text = column_name[i];
                    label.AutoSize = false;
                    label.Font = new Font("Calibri", 10, FontStyle.Bold);
                    label.Width = labelWidth;
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    label.Location = new Point(i * (labelWidth + spacing), yPos);
                    panel5.Controls.Add(label);
                }
            }

            TextBox firstSNoTextBox = null;
            AdjustControlPosition();
            yPos = LCyPos;
            // Create textboxes and comboboxes
            for (int row = 0; row < numRowsToAdd; row++)
            {
                TextBox surnameTextBox = null;
                TextBox fatherSurnameTextBox = null;
                for (int col = 0; col < column_name.Length; col++)
                {
                    Control control;

                    if (column_name[col] == "Form Type")
                    {
                        ComboBox formTypeComboBox = new ComboBox();
                        formTypeComboBox.Font = new Font("Calibri", 10);
                        formTypeComboBox.Width = textBoxWidth;
                        formTypeComboBox.Location = new Point(col * (labelWidth + spacing), yPos);
                        formTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                        formTypeComboBox.Items.AddRange(new string[] { "Title", "Other", "Blank", "Illegible", "Duplicate", "Birth_Baptism", "Marriage", "Death_Burial", "Page2", "Census", "Confirmation" });
                        panel3.Controls.Add(formTypeComboBox);
                        control = formTypeComboBox;
                        formTypeComboBox.SelectedIndexChanged += FormTypeComboBox_SelectedIndexChanged;
                        AdjustFieldVisibility(selectedFormType, formTypeFields, formTypeComboBox);
                    }
                    else if (column_name[col] == "Gender")
                    {
                        ComboBox genderComboBox = new ComboBox();
                        genderComboBox.Font = new Font("Calibri", 10);
                        genderComboBox.Width = textBoxWidth;
                        genderComboBox.Location = new Point(col * (labelWidth + spacing), yPos);
                        genderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                        genderComboBox.Items.AddRange(new string[] { "", "Male", "Female" });

                        genderComboBox.SelectedIndex = -1;
                        
                        panel3.Controls.Add(genderComboBox);
                        control = genderComboBox;
                    }
                    else
                    {
                        TextBox textBox = new TextBox();
                        textBox.Font = new Font("Calibri", 10);
                        textBox.Width = textBoxWidth;
                        textBox.Location = new Point(col * (labelWidth + spacing), yPos);
                        textBox.Click += TextBox_Click;
                        panel3.Controls.Add(textBox);
                        control = textBox;
                        textBoxCount++;

                       
                        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                        textBox.Click += TextBox_Click;
                    }
                    allTextBoxesAndLabels.Add(control);
                }
                yPos = yPosStart + (row + 1) * (rowHeight + spacing); // Position for next row
            }

            // Enable horizontal scrolling for both panels
            panel3.AutoScroll = true;
            // panel5.AutoScroll = true;
            panel3.HorizontalScroll.Enabled = true;
            panel3.HorizontalScroll.Visible = true;
            panel3.VerticalScroll.Enabled = false;
            panel3.VerticalScroll.Visible = false;
            
            panel5.AutoScroll = false;
            panel5.HorizontalScroll.Visible = false;
            panel3.Scroll += (sender, e) =>
            {
                int scrollValue = -panel3.AutoScrollPosition.X;
                panel5.AutoScrollPosition = new Point(scrollValue, 0);
            };

            foreach (Control control in panel3.Controls)
            {
                if (control is TextBox)
                {
                    control.GotFocus += (sender, e) =>
                    {
                        TextBox focusedTextBox = (TextBox)sender;
                        int y = -panel3.AutoScrollPosition.Y;
                        int targetX = focusedTextBox.Location.X - panel3.AutoScrollPosition.X - 840;
                        panel3.AutoScrollPosition = new Point(targetX, y);
                        int scrollValue = -panel3.AutoScrollPosition.X;
                        panel5.AutoScrollPosition = new Point(scrollValue, y);
                    };
                }
            }

            // Adjust the width of panel3 and panel5
            int totalWidth = column_name.Length * (labelWidth + spacing);
            //hyy
            //panel3.Width = totalWidth;
            //panel5.Width = totalWidth;

            // Adjust the height of panel3 to fit all controls
            int totalHeight = (numRowsToAdd + 1) * (rowHeight + spacing); // +1 to account for the header row
            panel3.AutoScrollMinSize = new Size(totalWidth, totalHeight);

            // Adjust panel5 height as needed
            //  panel5.AutoScrollMinSize = new Size(totalWidth, panel5.Height);

            if (firstSNoTextBox != null)
            {
                firstSNoTextBox.Focus();
            }

            return textBoxCount;
        }

        private bool IsValidDayForMonth(int day, int month)
        {
            // Days in each month (index 0 is not used)
            int[] daysInMonth = { 0, 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            if (month < 1 || month > 12)
            {
                return false; // Invalid month
            }

            // Check if the day is valid for the given month
            if (day < 1 || day > daysInMonth[month])
            {
                return false;
            }

            return true;
        }
        // Add a flag to track whether the error message has already been shown
        private bool hasShownError = false;

        private void BaptismYearTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Allow only digits and the question mark character
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '?' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Discard the input
                return;
            }

            // If the question mark is already present, prevent another one
            if (e.KeyChar == '?' && textBox.Text.Contains("?"))
            {
                e.Handled = true; // Discard the input
                return;
            }

            // Prevent entering more than 4 characters
            if (textBox.Text.Length >= 4 && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Discard the input
            }
        }

        private void BaptismYearTextBox_Leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Check if the input is exactly 4 digits long
            if (!hasShownError && (textBox.Text.Length != 4 || !textBox.Text.All(char.IsDigit)))
            {
                MessageBox.Show("Please enter exactly four digits.");
                textBox.Focus(); // Bring focus back to the TextBox for correction
                hasShownError = true; // Set the flag to prevent further messages
            }
            else if (textBox.Text.Length == 4 && textBox.Text.All(char.IsDigit))
            {
                // Reset the flag if the input is valid
                hasShownError = false;
            }
        }

        private void Panel_Scroll(object sender, ScrollEventArgs e)
        {
            if (sender == panel3)
            {
                int scrollValue = -panel3.AutoScrollPosition.X;
                panel5.AutoScrollPosition = new Point(scrollValue, 0);
            }
        }
       
        private string GetFieldNameFromControl(Control control)
        {
            string fieldName = string.Empty;

            if (control is TextBox || control is ComboBox)
            {
                var matchingLabel = panel5.Controls.OfType<Label>()
                    .FirstOrDefault(label => label.Location.X == control.Location.X);

                if (matchingLabel != null)
                {
                    fieldName = matchingLabel.Text;
                }
            }

            return fieldName;
        }
       
        private void FormTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;

            string selectedFormType = comboBox.SelectedItem.ToString();

            // Adjust the visibility of fields in the same row as the comboBox
            AdjustFieldVisibility(selectedFormType, formTypeFields, comboBox);
        }
        private void AdjustFieldVisibility(string selectedFormType, Dictionary<string, string[]> formTypeFields, Control triggeringControl = null)
        {
            // If triggeringControl is null, adjust all rows, otherwise just the specified row
            var controlsToAdjust = triggeringControl == null
                ? panel3.Controls.OfType<Control>()
                : panel3.Controls.OfType<Control>().Where(c => c.Location.Y == triggeringControl.Location.Y);

            foreach (var control in controlsToAdjust)
            {
                control.Visible = true;
            }

            if (formTypeFields.ContainsKey(selectedFormType))
            {
                var fieldsToShow = formTypeFields[selectedFormType];

                foreach (var control in controlsToAdjust)
                {
                    string fieldName = GetFieldNameFromControl(control);

                    if (!fieldsToShow.Contains(fieldName))
                    {
                        control.Visible = false;
                    }
                }
            }
        }

        //private void GenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ComboBox genderComboBox = sender as ComboBox;

        //    if (genderComboBox.SelectedIndex == 0) // Blank option selected
        //    {
        //        DialogResult result = MessageBox.Show("You have selected a blank option. Do you want to proceed?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        //        if (result == DialogResult.No)
        //        {
        //            // If the user selects "No", reset the ComboBox to the previous selection
        //            genderComboBox.SelectedIndex = -1; // Or set to the previous valid index
        //        }
        //    }
        //}


        //original
        //private void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    if (textBox == null) return;

        //    int tabIndex = panel3.Controls.IndexOf(textBox);
        //    int columnPerRow = 50;
        //    string outputDirectory = Path.Combine(path, "XML-Output");
        //    if (e.Shift && e.KeyCode == Keys.Tab)
        //    {
        //        if (tabIndex > 0)
        //        {
        //            Control previouscontrol = panel3.Controls[tabIndex - 1];
        //            previouscontrol.Focus();
        //        }
        //        e.IsInputKey = true;
        //    }
        //    else if (e.KeyCode == Keys.Tab)
        //    {
        //        int currentRow = tabIndex / columnPerRow;
        //        int lastUIDIndax = (currentRow + 1) * columnPerRow - 1;

        //        if (tabIndex == lastUIDIndax)
        //        {
        //            // Define all column names
        //            string[] column_name = {"SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Maiden Name", "Gender", "Age", "Birth day",
        //                   "Birth month", "Birth year", "Baptism day", "Baptism month", "Baptism year", "Death day", "Death month", "Death year", "Burial day",
        //                   "BurialMonth", "Burial Year", "Residence", "Father Given Name", "Father Surname", "Mother Given Name", "MotherSurname",
        //                   "MotherMaidenName", "Groom Given Name", "Groom Surname", "Groom Residence", "GroomFatherGivenName", "GroomFatherSurname",
        //                   "GroomMotherGivenName", "GroomMotherSurname", "GroomMotherMaidenName", "BrideGivenName", "BrideSurname", "BrideMaidenName",
        //                   "Bride Residence", "BrideFatherGiven_Name", "BrideFatherSurname", "BrideMotherGiven_Name", "BrideMotherSurname",
        //                   "BrideMotherMaidenName", "Marriageday", "Marriagemonth", "Marriageyear", "UID"};


        //            var controls = panel3.Controls.Cast<Control>();
        //            var row = controls.Skip(currentRow * columnPerRow).Take(columnPerRow)
        //                             .Select(c => c is TextBox Tb ? Tb.Text : (c is ComboBox cb ? cb.SelectedItem?.ToString() : ""))
        //                             .ToArray();


        //            if (row.Any(VALUE => !string.IsNullOrEmpty(VALUE)))
        //            {
        //                // string outputDirectory = Path.Combine(path, "XML-Output");
        //                if (Directory.Exists(outputDirectory))
        //                {
        //                    Directory.CreateDirectory(outputDirectory);
        //                }

        //                // Define the path for the XML file
        //                string currentImageName = current_image.ToString().Split('.').First();
        //                string xmlFilePath = Path.Combine(outputDirectory, $"{currentImageName}.xml");
        //            }

        //            // Load existing XML or create a new one
        //            XDocument doc;
        //            if (File.Exists(xmlFilePath))
        //            {
        //                doc = XDocument.Load(xmlFilePath);
        //            }
        //            else
        //            {
        //                doc = new XDocument(new XElement("Root"));
        //            }

        //            // Find the existing element to update or create a new one
        //            XElement existingRow = doc.Root.Elements("Row")
        //                .FirstOrDefault(r => r.Attribute("SNo")?.Value == row.ElementAtOrDefault(0));

        //            if (existingRow != null)
        //            {
        //                // Update existing row attributes
        //                UpdateAttributes(existingRow, row);
        //            }
        //            else
        //            {
        //                XElement newRow = new XElement("Row",
        //                     new XAttribute("SNo", row.ElementAtOrDefault(0) ?? ""),
        //                     new XAttribute("FolderID", row.ElementAtOrDefault(1) ?? ""),
        //                     new XAttribute("ImageID", row.ElementAtOrDefault(2) ?? ""),
        //                     new XAttribute("FormType", row.ElementAtOrDefault(3) ?? ""),
        //                     new XAttribute("LineNumber", row.ElementAtOrDefault(4) ?? ""),

        //                     new XAttribute("MarriageYear", row.ElementAtOrDefault(48) ?? ""),
        //                     new XAttribute("UID", row.ElementAtOrDefault(49) ?? ""),
        //                     new XAttribute("CurrentDate", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"))
        //                 );
        //                doc.Root.Add(newRow);
        //            }
        //            try
        //            {
        //                if (xmlFilePath != null) // Ensure xmlFilePath is not null before saving
        //                {
        //                    doc.Save(xmlFilePath);
        //                    MessageBox.Show("Row saved successfully");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error saving XML file: {ex.Message}");
        //            }

        //            // Calculate total count for records with today's date
        //            string todayDate = DateTime.Now.ToString("dd/MM/yy");
        //            int totalCount = 0;
        //            var xmlFiles = Directory.GetFiles(outputDirectory, "*.xml");

        //            foreach (var xmlFile in xmlFiles)
        //            {
        //                try
        //                {
        //                    XDocument xdoc = XDocument.Load(xmlFile);
        //                    var count = xdoc.Descendants("Row")
        //                        .Where(r => r.Attribute("CurrentDate") != null &&
        //                                    r.Attribute("CurrentDate").Value.StartsWith(todayDate))
        //                        .Count();
        //                    totalCount += count;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"Error processing XML file '{xmlFile}': {ex.Message}");
        //                }
        //            }

        //            label3.Text = totalCount.ToString();
        //        }
        //    }
        //}


        //private void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    if (textBox == null) return;

        //    int tabIndex = panel3.Controls.IndexOf(textBox);
        //    int columnPerRow = 50; // Assuming you have 50 columns per row
        //    string outputDirectory = Path.Combine(path, "XML-Output");

        //    if (e.Shift && e.KeyCode == Keys.Tab)
        //    {
        //        if (tabIndex > 0)
        //        {
        //            Control previousControl = panel3.Controls[tabIndex - 1];
        //            previousControl.Focus();
        //        }
        //        e.IsInputKey = true;
        //    }
        //    else if (e.KeyCode == Keys.Tab)
        //    {
        //        int currentRow = tabIndex / columnPerRow;
        //        int lastUIDIndex = (currentRow + 1) * columnPerRow - 1;

        //        if (tabIndex == lastUIDIndex)
        //        {
        //            SaveRowDataToXml(currentRow, outputDirectory);
        //        }
        //    }
        //}

        //private void SaveRowDataToXml(int currentRow, string outputDirectory)
        //{
        //    int columnPerRow = 50;
        //    var controls = panel3.Controls.Cast<Control>();
        //    var row = controls.Skip(currentRow * columnPerRow)
        //                       .Take(columnPerRow)
        //                       .Select(c => c is TextBox tb ? tb.Text : (c is ComboBox cb ? cb.SelectedItem?.ToString() : ""))
        //                       .ToArray();

        //    string currentImageName = current_image.ToString().Split('.').First();
        //    string xmlFilePath = Path.Combine(outputDirectory, $"{currentImageName}.xml");

        //    XDocument doc = File.Exists(xmlFilePath) ? XDocument.Load(xmlFilePath) : new XDocument(new XElement("Root"));

        //    XElement existingRow = doc.Root.Elements("Row")
        //        .FirstOrDefault(r => r.Attribute("SNo")?.Value == row.ElementAtOrDefault(0));

        //    if (existingRow != null)
        //    {
        //        UpdateAttributes(existingRow, row);
        //    }
        //    else
        //    {


        //        XElement newRow = new XElement("Row",
        //             new XAttribute("SNo", row.ElementAtOrDefault(0) ?? ""),
        //             new XAttribute("FolderID", row.ElementAtOrDefault(1) ?? ""),
        //             new XAttribute("ImageID", row.ElementAtOrDefault(2) ?? ""),
        //             new XAttribute("FormType", row.ElementAtOrDefault(3) ?? ""),
        //             new XAttribute("LineNumber", row.ElementAtOrDefault(4) ?? ""),
        //             new XAttribute("Page", row.ElementAtOrDefault(5) ?? ""),
        //             new XAttribute("GivenName", row.ElementAtOrDefault(6) ?? ""),
        //             new XAttribute("Surname", row.ElementAtOrDefault(7) ?? ""),
        //             new XAttribute("MaidenName", row.ElementAtOrDefault(8) ?? ""),
        //             new XAttribute("Gender", row.ElementAtOrDefault(9) ?? ""),
        //             new XAttribute("Age", row.ElementAtOrDefault(10) ?? ""),
        //             new XAttribute("BirthDay", row.ElementAtOrDefault(11) ?? ""),
        //             new XAttribute("BirthMonth", row.ElementAtOrDefault(12) ?? ""),
        //             new XAttribute("BirthYear", row.ElementAtOrDefault(13) ?? ""),
        //             new XAttribute("BaptismDay", row.ElementAtOrDefault(14) ?? ""),
        //             new XAttribute("BaptismMonth", row.ElementAtOrDefault(15) ?? ""),
        //             new XAttribute("BaptismYear", row.ElementAtOrDefault(16) ?? ""),
        //             new XAttribute("DeathDay", row.ElementAtOrDefault(17) ?? ""),
        //             new XAttribute("DeathMonth", row.ElementAtOrDefault(18) ?? ""),
        //             new XAttribute("DeathYear", row.ElementAtOrDefault(19) ?? ""),
        //             new XAttribute("BurialDay", row.ElementAtOrDefault(20) ?? ""),
        //             new XAttribute("BurialMonth", row.ElementAtOrDefault(21) ?? ""),
        //             new XAttribute("BurialYear", row.ElementAtOrDefault(22) ?? ""),
        //             new XAttribute("Residence", row.ElementAtOrDefault(23) ?? ""),
        //             new XAttribute("FatherGivenName", row.ElementAtOrDefault(24) ?? ""),
        //             new XAttribute("FatherSurname", row.ElementAtOrDefault(25) ?? ""),
        //             new XAttribute("MotherGivenName", row.ElementAtOrDefault(26) ?? ""),
        //             new XAttribute("MotherSurname", row.ElementAtOrDefault(27) ?? ""),
        //             new XAttribute("MotherMaidenName", row.ElementAtOrDefault(28) ?? ""),
        //             new XAttribute("GroomGivenName", row.ElementAtOrDefault(29) ?? ""),
        //             new XAttribute("GroomSurname", row.ElementAtOrDefault(30) ?? ""),
        //             new XAttribute("GroomResidence", row.ElementAtOrDefault(31) ?? ""),
        //             new XAttribute("GroomFatherGivenName", row.ElementAtOrDefault(32) ?? ""),
        //             new XAttribute("GroomFatherSurname", row.ElementAtOrDefault(33) ?? ""),
        //             new XAttribute("GroomMotherGivenName", row.ElementAtOrDefault(34) ?? ""),
        //             new XAttribute("GroomMotherSurname", row.ElementAtOrDefault(35) ?? ""),
        //             new XAttribute("GroomMotherMaidenName", row.ElementAtOrDefault(36) ?? ""),
        //             new XAttribute("BrideGivenName", row.ElementAtOrDefault(37) ?? ""),
        //             new XAttribute("BrideSurname", row.ElementAtOrDefault(38) ?? ""),
        //             new XAttribute("BrideMaidenName", row.ElementAtOrDefault(39) ?? ""),
        //             new XAttribute("BrideResidence", row.ElementAtOrDefault(40) ?? ""),
        //             new XAttribute("BrideFatherGivenName", row.ElementAtOrDefault(41) ?? ""),
        //             new XAttribute("BrideFatherSurname", row.ElementAtOrDefault(42) ?? ""),
        //             new XAttribute("BrideMotherGivenName", row.ElementAtOrDefault(43) ?? ""),
        //             new XAttribute("BrideMotherSurname", row.ElementAtOrDefault(44) ?? ""),
        //             new XAttribute("BrideMotherMaidenName", row.ElementAtOrDefault(45) ?? ""),
        //             new XAttribute("MarriageDay", row.ElementAtOrDefault(46) ?? ""),
        //             new XAttribute("MarriageMonth", row.ElementAtOrDefault(47) ?? ""),
        //             new XAttribute("MarriageYear", row.ElementAtOrDefault(48) ?? ""),
        //             new XAttribute("UID", row.ElementAtOrDefault(49) ?? ""),
        //            new XAttribute("CurrentDate", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"))
        //         );
        //        doc.Root.Add(newRow);
        //    }

        //    try
        //    {
        //        if (xmlFilePath != null) // Ensure xmlFilePath is not null before saving
        //        {
        //            doc.Save(xmlFilePath);
        //            MessageBox.Show("Row saved successfully");
        //            UpdateTotalCount(outputDirectory);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error saving XML file: {ex.Message}");
        //    }
        //}

        //private void UpdateTotalCount(string outputDirectory)
        //{
        //    string todayDate = DateTime.Now.ToString("dd/MM/yy");
        //    int totalCount = 0;
        //    var xmlFiles = Directory.GetFiles(outputDirectory, "*.xml");

        //    foreach (var xmlFile in xmlFiles)
        //    {
        //        try
        //        {
        //            XDocument xdoc = XDocument.Load(xmlFile);
        //            var count = xdoc.Descendants("Row")
        //                .Where(r => r.Attribute("CurrentDate") != null &&
        //                            r.Attribute("CurrentDate").Value.StartsWith(todayDate))
        //                .Count();
        //            totalCount += count;
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error processing XML file '{xmlFile}': {ex.Message}");
        //        }
        //    }

        //    label3.Text = totalCount.ToString();
        //}

        private void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox == null) return;

            int tabIndex = panel3.Controls.IndexOf(textBox);
            int columnPerRow = 55;
            string outputDirectory = Path.Combine(path, "XML-Output");

            if (e.KeyCode == Keys.Tab)
            {
                int currentRow = tabIndex / columnPerRow;
                int lastNOTESIndex = (currentRow + 1) * columnPerRow - 1;

                if (tabIndex == lastNOTESIndex)
                {
                    // Define all column names
                    string[] columnNames = {"SNo","Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Maiden Name", "Gender", "Age", "Birth day",
                       "Birth month", "Birth year", "Baptism day", "Baptism month", "Baptism year", "Death day", "Death month", "Death year", "Burial day",
                       "Burial month", "Burial Year", "Residence", "Father Given Name", "Father Surname", "Mother Given Name", "Mother Surname",
                       "Mother Maiden Name", "Groom Given Name", "Groom Surname", "Groom Residence", "Groom Father Given Name", "Groom Father Surname",
                       "Groom Mother Given Name", "Groom Mother Surname", "Groom Mother Maiden Name", "Bride Given Name", "Bride Surname", "Bride Maiden Name",
                       "Bride Residence", "Bride Father Given_Name", "Bride Father Surname", "Bride Mother Given_Name", "Bride Mother Surname",
                       "Bride Mother Maiden Name", "Marriage day", "Marriage month", "Marriage year","Census year","Confirmation day","Confirmation month","Confirmation year", "UID","NOTES"};
                    var controls = panel3.Controls.Cast<Control>();
                    var row = controls.Skip(currentRow * columnPerRow).Take(columnPerRow)
                                       .Select(c => c is TextBox tb ? tb.Text : (c is ComboBox cb ? cb.SelectedItem?.ToString() : ""))
                                       .ToArray();

                    // Ensure the output directory exists
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }

                    // Define the path for the XML file
                    string currentImageName = current_image.ToString().Split('.').First();
                    string xmlFilePath = Path.Combine(outputDirectory, $"{currentImageName}.xml");

                    // Load existing XML or create a new one
                    XDocument doc;
                    if (File.Exists(xmlFilePath))
                    {
                        doc = XDocument.Load(xmlFilePath);
                    }
                    else
                    {
                        doc = new XDocument(new XElement("Root"));
                    }

                    // Find the existing element to update or create a new one
                    XElement existingRow = doc.Root.Elements("Row")
                        .FirstOrDefault(r => r.Attribute("SNo")?.Value == row.ElementAtOrDefault(0));

                    if (existingRow != null)
                    {
                        // Update existing row attributes
                        UpdateAttributes(existingRow, row);
                    }
                    else
                    {
                        XElement newRow = new XElement("Row",
                           new XAttribute("SNo", row.ElementAtOrDefault(0) ?? ""),
                          new XAttribute("FolderID", row.ElementAtOrDefault(1) ?? ""),
                          new XAttribute("ImageID", row.ElementAtOrDefault(2) ?? ""),
                          new XAttribute("FormType", row.ElementAtOrDefault(3) ?? ""),
                          new XAttribute("LineNumber", row.ElementAtOrDefault(4) ?? ""),
                          new XAttribute("Page", row.ElementAtOrDefault(5) ?? ""),
                          new XAttribute("GivenName", row.ElementAtOrDefault(6) ?? ""),
                          new XAttribute("Surname", row.ElementAtOrDefault(7) ?? ""),
                          new XAttribute("MaidenName", row.ElementAtOrDefault(8) ?? ""),
                          new XAttribute("Gender", row.ElementAtOrDefault(9) ?? ""),
                          new XAttribute("Age", row.ElementAtOrDefault(10) ?? ""),
                          new XAttribute("BirthDay", row.ElementAtOrDefault(11) ?? ""),
                          new XAttribute("BirthMonth", row.ElementAtOrDefault(12) ?? ""),
                          new XAttribute("BirthYear", row.ElementAtOrDefault(13) ?? ""),
                          new XAttribute("BaptismDay", row.ElementAtOrDefault(14) ?? ""),
                          new XAttribute("BaptismMonth", row.ElementAtOrDefault(15) ?? ""),
                          new XAttribute("BaptismYear", row.ElementAtOrDefault(16) ?? ""),
                          new XAttribute("DeathDay", row.ElementAtOrDefault(17) ?? ""),
                          new XAttribute("DeathMonth", row.ElementAtOrDefault(18) ?? ""),
                          new XAttribute("DeathYear", row.ElementAtOrDefault(19) ?? ""),
                          new XAttribute("BurialDay", row.ElementAtOrDefault(20) ?? ""),
                          new XAttribute("BurialMonth", row.ElementAtOrDefault(21) ?? ""),
                          new XAttribute("BurialYear", row.ElementAtOrDefault(22) ?? ""),
                          new XAttribute("Residence", row.ElementAtOrDefault(23) ?? ""),
                          new XAttribute("FatherGivenName", row.ElementAtOrDefault(24) ?? ""),
                          new XAttribute("FatherSurname", row.ElementAtOrDefault(25) ?? ""),
                          new XAttribute("MotherGivenName", row.ElementAtOrDefault(26) ?? ""),
                          new XAttribute("MotherSurname", row.ElementAtOrDefault(27) ?? ""),
                          new XAttribute("MotherMaidenName", row.ElementAtOrDefault(28) ?? ""),
                          new XAttribute("GroomGivenName", row.ElementAtOrDefault(29) ?? ""),
                          new XAttribute("GroomSurname", row.ElementAtOrDefault(30) ?? ""),
                          new XAttribute("GroomResidence", row.ElementAtOrDefault(31) ?? ""),
                          new XAttribute("GroomFatherGivenName", row.ElementAtOrDefault(32) ?? ""),
                          new XAttribute("GroomFatherSurname", row.ElementAtOrDefault(33) ?? ""),
                          new XAttribute("GroomMotherGivenName", row.ElementAtOrDefault(34) ?? ""),
                          new XAttribute("GroomMotherSurname", row.ElementAtOrDefault(35) ?? ""),
                          new XAttribute("GroomMotherMaidenName", row.ElementAtOrDefault(36) ?? ""),
                          new XAttribute("BrideGivenName", row.ElementAtOrDefault(37) ?? ""),
                          new XAttribute("BrideSurname", row.ElementAtOrDefault(38) ?? ""),
                          new XAttribute("BrideMaidenName", row.ElementAtOrDefault(39) ?? ""),
                          new XAttribute("BrideResidence", row.ElementAtOrDefault(40) ?? ""),
                          new XAttribute("BrideFatherGivenName", row.ElementAtOrDefault(41) ?? ""),
                          new XAttribute("BrideFatherSurname", row.ElementAtOrDefault(42) ?? ""),
                          new XAttribute("BrideMotherGivenName", row.ElementAtOrDefault(43) ?? ""),
                          new XAttribute("BrideMotherSurname", row.ElementAtOrDefault(44) ?? ""),
                          new XAttribute("BrideMotherMaidenName", row.ElementAtOrDefault(45) ?? ""),
                          new XAttribute("MarriageDay", row.ElementAtOrDefault(46) ?? ""),
                          new XAttribute("MarriageMonth", row.ElementAtOrDefault(47) ?? ""),
                          new XAttribute("MarriageYear", row.ElementAtOrDefault(48) ?? ""),
                          new XAttribute("CensusYear", row.ElementAtOrDefault(49) ?? ""),
                          new XAttribute("ConfirmationDay", row.ElementAtOrDefault(50) ?? ""),
                          new XAttribute("ConfirmationMonth", row.ElementAtOrDefault(51) ?? ""),
                          new XAttribute("ConfirmationYear", row.ElementAtOrDefault(52) ?? ""),
                          new XAttribute("UID", row.ElementAtOrDefault(53) ?? ""),
                          new XAttribute("NOTES", row.ElementAtOrDefault(54) ?? ""),
                          new XAttribute("CurrentDate", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"))
                          );
                        doc.Root.Add(newRow);
                    }

                    try
                    {
                        if (!string.IsNullOrEmpty(xmlFilePath))
                        {
                            doc.Save(xmlFilePath);
                            MessageBox.Show("Row saved successfully");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving XML file: {ex.Message}");
                    }

                    // Calculate total count for records with today's date
                    string todayDate = DateTime.Now.ToString("dd/MM/yy");
                    int totalCount = 0;
                    var xmlFiles = Directory.GetFiles(outputDirectory, "*.xml");

                    foreach (var xmlFile in xmlFiles)
                    {
                        try
                        {
                            XDocument xdoc = XDocument.Load(xmlFile);
                            var count = xdoc.Descendants("Row")
                                .Count(r => r.Attribute("CurrentDate") != null &&
                                            r.Attribute("CurrentDate").Value.StartsWith(todayDate));
                            totalCount += count;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing XML file '{xmlFile}': {ex.Message}");
                        }
                    }
                    label3.Text = totalCount.ToString();
                }
            }
        }

        private void HandleCtrlD()
        {
            TextBox focusedTextBox = panel3.Controls
                .OfType<TextBox>()
                .FirstOrDefault(t => t.Focused);

            int textBoxIndex = panel3.Controls.IndexOf(focusedTextBox);
            int indexis = textBoxIndex - 55;
            Control reTextBox = panel3.Controls[indexis];

            focusedTextBox.Text = reTextBox.Text;
        }

        //private void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    TextBox textBox = sender as TextBox;
        //    if (textBox == null) return;

        //    int tabIndex = panel3.Controls.IndexOf(textBox);
        //    int columnPerRow = 55;
        //    string outputDirectory = Path.Combine(path, "XML-Output");

        //    if (e.KeyCode == Keys.Tab)
        //    {
        //        // Get the current row based on the tab index
        //        int currentRow = tabIndex / columnPerRow;
        //        int lastNOTESIndex = (currentRow + 1) * columnPerRow - 1;

        //        // Skip remaining fields if the form type is one of the following values
        //        ComboBox formTypeComboBox = panel3.Controls.OfType<ComboBox>()
        //            .Where(cb => panel3.Controls.IndexOf(cb) == (currentRow * columnPerRow + 3)) // Index for FormType
        //            .FirstOrDefault();

        //        if (formTypeComboBox != null &&
        //            new[] { "Title", "Other", "Blank", "Illegible", "Duplicate" }.Contains(formTypeComboBox.SelectedItem?.ToString()))
        //        {
        //            // Move directly to the "NOTES" field
        //            TextBox notesTextBox = panel3.Controls.OfType<TextBox>()
        //                .Where(tb => panel3.Controls.IndexOf(tb) == lastNOTESIndex)
        //                .FirstOrDefault();

        //            if (notesTextBox != null)
        //            {
        //                notesTextBox.Focus();
        //                e.IsInputKey = true;
        //            }
        //        }

        //        // Handle saving when the last field (NOTES) is reached
        //        if (tabIndex == lastNOTESIndex)
        //        {
        //            string[] columnNames = { "SNo", "Folder ID", "Image ID", "Form Type", "Line Number", "Page", "Given Name", "Surname", "Maiden Name", "Gender", "Age", "Birth day",
        //       "Birth month", "Birth year", "Baptism day", "Baptism month", "Baptism year", "Death day", "Death month", "Death year", "Burial day",
        //       "Burial month", "Burial Year", "Residence", "Father Given Name", "Father Surname", "Mother Given Name", "Mother Surname",
        //       "Mother Maiden Name", "Groom Given Name", "Groom Surname", "Groom Residence", "Groom Father Given Name", "Groom Father Surname",
        //       "Groom Mother Given Name", "Groom Mother Surname", "Groom Mother Maiden Name", "Bride Given Name", "Bride Surname", "Bride Maiden Name",
        //       "Bride Residence", "Bride Father Given_Name", "Bride Father Surname", "Bride Mother Given_Name", "Bride Mother Surname",
        //       "Bride Mother Maiden Name", "Marriage day", "Marriage month", "Marriage year","Census year","Confirmation day","Confirmation month","Confirmation year", "UID","NOTES" };

        //            var controls = panel3.Controls.Cast<Control>();
        //            var row = controls.Skip(currentRow * columnPerRow).Take(columnPerRow)
        //                               .Select(c => c is TextBox tb ? tb.Text : (c is ComboBox cb ? cb.SelectedItem?.ToString() : ""))
        //                               .ToArray();

        //            // Ensure the output directory exists
        //            if (!Directory.Exists(outputDirectory))
        //            {
        //                Directory.CreateDirectory(outputDirectory);
        //            }

        //            // Define the path for the XML file
        //            string currentImageName = current_image.ToString().Split('.').First();
        //            string xmlFilePath = Path.Combine(outputDirectory, $"{currentImageName}.xml");

        //            // Load existing XML or create a new one
        //            XDocument doc;
        //            if (File.Exists(xmlFilePath))
        //            {
        //                doc = XDocument.Load(xmlFilePath);
        //            }
        //            else
        //            {
        //                doc = new XDocument(new XElement("Root"));
        //            }

        //            // Find the existing element to update or create a new one
        //            XElement existingRow = doc.Root.Elements("Row")
        //                .FirstOrDefault(r => r.Attribute("SNo")?.Value == row.ElementAtOrDefault(0));

        //            if (existingRow != null)
        //            {
        //                // Update existing row attributes
        //                UpdateAttributes(existingRow, row);
        //            }
        //            else
        //            {
        //                XElement newRow = new XElement("Row",
        //                    new XAttribute("SNo", row.ElementAtOrDefault(0) ?? ""),
        //                    new XAttribute("FolderID", row.ElementAtOrDefault(1) ?? ""),
        //                    new XAttribute("ImageID", row.ElementAtOrDefault(2) ?? ""),
        //                    new XAttribute("FormType", row.ElementAtOrDefault(3) ?? ""),
        //                    new XAttribute("LineNumber", row.ElementAtOrDefault(4) ?? ""),
        //                    new XAttribute("Page", row.ElementAtOrDefault(5) ?? ""),
        //                    new XAttribute("GivenName", row.ElementAtOrDefault(6) ?? ""),
        //                    new XAttribute("Surname", row.ElementAtOrDefault(7) ?? ""),
        //                    new XAttribute("MaidenName", row.ElementAtOrDefault(8) ?? ""),
        //                    new XAttribute("Gender", row.ElementAtOrDefault(9) ?? ""),
        //                    new XAttribute("Age", row.ElementAtOrDefault(10) ?? ""),
        //                    // Add remaining attributes...
        //                    new XAttribute("NOTES", row.ElementAtOrDefault(54) ?? ""),
        //                    new XAttribute("CurrentDate", DateTime.Now.ToString("dd/MM/yy HH:mm:ss"))
        //                );
        //                doc.Root.Add(newRow);
        //            }

        //            try
        //            {
        //                if (!string.IsNullOrEmpty(xmlFilePath))
        //                {
        //                    doc.Save(xmlFilePath);
        //                    MessageBox.Show("Row saved successfully");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show($"Error saving XML file: {ex.Message}");
        //            }

        //            // Calculate total count for records with today's date
        //            string todayDate = DateTime.Now.ToString("dd/MM/yy");
        //            int totalCount = 0;
        //            var xmlFiles = Directory.GetFiles(outputDirectory, "*.xml");

        //            foreach (var xmlFile in xmlFiles)
        //            {
        //                try
        //                {
        //                    XDocument xdoc = XDocument.Load(xmlFile);
        //                    var count = xdoc.Descendants("Row")
        //                        .Count(r => r.Attribute("CurrentDate") != null &&
        //                                    r.Attribute("CurrentDate").Value.StartsWith(todayDate));
        //                    totalCount += count;
        //                }
        //                catch (Exception ex)
        //                {
        //                    Console.WriteLine($"Error processing XML file '{xmlFile}': {ex.Message}");
        //                }
        //            }
        //            label3.Text = totalCount.ToString();
        //        }
        //    }
        //}


        private void UpdateAttributes(XElement existingRow, string[] row)
        {
            existingRow.SetAttributeValue("SNo", row.ElementAtOrDefault(0) ?? "");
            existingRow.SetAttributeValue("FolderID", row.ElementAtOrDefault(1) ?? "");
            existingRow.SetAttributeValue("ImageID", row.ElementAtOrDefault(2) ?? "");
            existingRow.SetAttributeValue("FormType", row.ElementAtOrDefault(3) ?? "");
            existingRow.SetAttributeValue("LineNumber", row.ElementAtOrDefault(4) ?? "");
            existingRow.SetAttributeValue("Page", row.ElementAtOrDefault(5) ?? "");
            existingRow.SetAttributeValue("GivenName", row.ElementAtOrDefault(6) ?? "");
            existingRow.SetAttributeValue("Surname", row.ElementAtOrDefault(7) ?? "");
            existingRow.SetAttributeValue("MaidenName", row.ElementAtOrDefault(8) ?? "");
            existingRow.SetAttributeValue("Gender", row.ElementAtOrDefault(9) ?? "");
            existingRow.SetAttributeValue("Age", row.ElementAtOrDefault(10) ?? "");
            existingRow.SetAttributeValue("BirthDay", row.ElementAtOrDefault(11) ?? "");
            existingRow.SetAttributeValue("BirthMonth", row.ElementAtOrDefault(12) ?? "");
            existingRow.SetAttributeValue("BirthYear", row.ElementAtOrDefault(13) ?? "");
            existingRow.SetAttributeValue("BaptismDay", row.ElementAtOrDefault(14) ?? "");
            existingRow.SetAttributeValue("BaptismMonth", row.ElementAtOrDefault(15) ?? "");
            existingRow.SetAttributeValue("BaptismYear", row.ElementAtOrDefault(16) ?? "");
            existingRow.SetAttributeValue("DeathDay", row.ElementAtOrDefault(17) ?? "");
            existingRow.SetAttributeValue("DeathMonth", row.ElementAtOrDefault(18) ?? "");
            existingRow.SetAttributeValue("DeathYear", row.ElementAtOrDefault(19) ?? "");
            existingRow.SetAttributeValue("BurialDay", row.ElementAtOrDefault(20) ?? "");
            existingRow.SetAttributeValue("BurialMonth", row.ElementAtOrDefault(21) ?? "");
            existingRow.SetAttributeValue("BurialYear", row.ElementAtOrDefault(22) ?? "");
            existingRow.SetAttributeValue("Residence", row.ElementAtOrDefault(23) ?? "");
            existingRow.SetAttributeValue("FatherGivenName", row.ElementAtOrDefault(24) ?? "");
            existingRow.SetAttributeValue("FatherSurname", row.ElementAtOrDefault(25) ?? "");
            existingRow.SetAttributeValue("MotherGivenName", row.ElementAtOrDefault(26) ?? "");
            existingRow.SetAttributeValue("MotherSurname", row.ElementAtOrDefault(27) ?? "");
            existingRow.SetAttributeValue("MotherMaidenName", row.ElementAtOrDefault(28) ?? "");
            existingRow.SetAttributeValue("GroomGivenName", row.ElementAtOrDefault(29) ?? "");
            existingRow.SetAttributeValue("GroomSurname", row.ElementAtOrDefault(30) ?? "");
            existingRow.SetAttributeValue("GroomResidence", row.ElementAtOrDefault(31) ?? "");
            existingRow.SetAttributeValue("GroomFatherGivenName", row.ElementAtOrDefault(32) ?? "");
            existingRow.SetAttributeValue("GroomFatherSurname", row.ElementAtOrDefault(33) ?? "");
            existingRow.SetAttributeValue("GroomMotherGivenName", row.ElementAtOrDefault(34) ?? "");
            existingRow.SetAttributeValue("GroomMotherSurname", row.ElementAtOrDefault(35) ?? "");
            existingRow.SetAttributeValue("GroomMotherMaidenName", row.ElementAtOrDefault(36) ?? "");
            existingRow.SetAttributeValue("BrideGivenName", row.ElementAtOrDefault(37) ?? "");
            existingRow.SetAttributeValue("BrideSurname", row.ElementAtOrDefault(38) ?? "");
            existingRow.SetAttributeValue("BrideMaidenName", row.ElementAtOrDefault(39) ?? "");
            existingRow.SetAttributeValue("BrideResidence", row.ElementAtOrDefault(40) ?? "");
            existingRow.SetAttributeValue("BrideFatherGivenName", row.ElementAtOrDefault(41) ?? "");
            existingRow.SetAttributeValue("BrideFatherSurname", row.ElementAtOrDefault(42) ?? "");
            existingRow.SetAttributeValue("BrideMotherGivenName", row.ElementAtOrDefault(43) ?? "");
            existingRow.SetAttributeValue("BrideMotherSurname", row.ElementAtOrDefault(44) ?? "");
            existingRow.SetAttributeValue("BrideMotherMaidenName", row.ElementAtOrDefault(45) ?? "");
            existingRow.SetAttributeValue("MarriageDay", row.ElementAtOrDefault(46) ?? "");
            existingRow.SetAttributeValue("MarriageMonth", row.ElementAtOrDefault(47) ?? "");
            existingRow.SetAttributeValue("MarriageYear", row.ElementAtOrDefault(48) ?? "");
            existingRow.SetAttributeValue("CensusYear", row.ElementAtOrDefault(49) ?? "");
            existingRow.SetAttributeValue("ConfirmationDay", row.ElementAtOrDefault(50) ?? "");
            existingRow.SetAttributeValue("ConfirmationMonth", row.ElementAtOrDefault(51) ?? "");
            existingRow.SetAttributeValue("ConfirmationYear", row.ElementAtOrDefault(52) ?? "");
            existingRow.SetAttributeValue("UID", row.ElementAtOrDefault(53) ?? "");
            existingRow.SetAttributeValue("NOTES", row.ElementAtOrDefault(54) ?? "");
        }

        //private void btn_del_Click(object sender, EventArgs e)
        //{
        //    var del_text = txt_del.Text;
        //    if(!string.IsNullOrEmpty(del_text))
        //    {
        //        DialogResult result = MessageBox.Show($"Delete Record No '{del_text}'", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

        //        DeleteRowFromXML(del_text);

        //        DeleteRowFromPanel(del_text);

        //        MessageBox.Show("Record deleted successfully!");
        //    }
        //}

        //private void DeleteRowFromXML(string recordsNumber)
        //{
        //    string outputDirectory = Path.Combine(path, "XML-Output");
        //    string xmlFilepath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(current_image.Name)}.xml");

        //    if(File.Exists(xmlFilepath))
        //    {
        //        XDocument doc = XDocument.Load(xmlFilepath);    

        //        XElement rowToDelete = doc.Descendants("Row")
        //                                   .FirstOrDefault(r=>(string)r.Attribute("SNo")== recordsNumber);

        //        if(rowToDelete != null)
        //        {
        //            rowToDelete.Remove();
        //            doc.Save(xmlFilepath);

        //            MessageBox.Show($"Record {recordsNumber} deleted successfully from XML file.");
        //        }
        //        else
        //        {
        //            MessageBox.Show($"Record {recordsNumber}not found in the xml file.");
        //        }
        //    }
        //    else
        //    {
        //        MessageBox.Show($"XML file not fount at path:{xmlFilepath}");
        //    }
        //}

        //private void DeleteRowFromPanel(string recordsNumber)
        //{
        //    try
        //    {
        //        var controlsToDelete = panel3.Controls.OfType<TextBox>().Where(tb => tb.Text == recordsNumber).ToList();

        //        if (controlsToDelete.Count > 0)
        //        {
        //            var yPos = controlsToDelete.First().Location.Y;
        //            var rowControls = panel3.Controls.Cast<Control>()
        //                                            .Where(c => c.Location.Y == yPos).ToList();

        //            foreach (var Control in rowControls)
        //            {
        //                panel3.Controls.Remove(Control);
        //            }
        //            AdjustControlPosition();

        //        }
        //        else
        //        {
        //            MessageBox.Show($"No TextBox found with test '{recordsNumber}'.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"An error occurred while deleting from the panel:{ex.Message}");
        //    }

        //}

        //private void AdjustControlPosition()
        //{
        //    int labelWidth = 175;
        //    int testBoxWith = 175;
        //    int rowHeight = 40;
        //    int spacing = 3;
        //    int yPos = 0;

        //    foreach (var rowControls in panel3.Controls.Cast<Control>().GroupBy(c =>c.Location.Y).OrderBy(g=>g.Key))
        //    {
        //        foreach (var Control in rowControls)
        //        {
        //            Control.Location = new Point(Control.Location.X, yPos);

        //        }
        //        yPos += rowHeight + spacing;                
        //    }
        //    LCyPos = yPos;
        //    panel3.AutoScroll = true;   
        //    panel3.AutoScrollMinSize =new Size(0,yPos); 
        //}

        private void btn_del_Click(object sender, EventArgs e)
        {
            var del_text = txt_del.Text.Trim(); // Trim any extra spaces
            if (!string.IsNullOrEmpty(del_text))
            {
                DialogResult result = MessageBox.Show($"Delete Record No '{del_text}'", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK) // Ensure user confirmed the deletion
                {
                    DeleteRowFromXML(del_text);
                    DeleteRowFromPanel(del_text);
                    MessageBox.Show("Record deleted successfully!");
                }
            }
        }

        private void DeleteRowFromXML(string recordsNumber)
        {
            string outputDirectory = Path.Combine(path, "XML-Output");
            string xmlFilepath = Path.Combine(outputDirectory, $"{Path.GetFileNameWithoutExtension(current_image.Name)}.xml");

            if (File.Exists(xmlFilepath))
            {
                XDocument doc = XDocument.Load(xmlFilepath);

                XElement rowToDelete = doc.Descendants("Row")
                                          .FirstOrDefault(r => (string)r.Attribute("SNo") == recordsNumber);

                if (rowToDelete != null)
                {
                    rowToDelete.Remove();
                    doc.Save(xmlFilepath);
                    MessageBox.Show($"Record {recordsNumber} deleted successfully from XML file.");
                }
                else
                {
                    MessageBox.Show($"Record {recordsNumber} not found in the XML file.");
                }
            }
            else
            {
                MessageBox.Show($"XML file not found at path: {xmlFilepath}");
            }
        }

        private void DeleteRowFromPanel(string recordsNumber)
        {
            try
            {
                var controlsToDelete = panel3.Controls.OfType<TextBox>()
                                      .Where(tb => tb.Text.Trim() == recordsNumber) // Trim text for accurate matching
                                      .ToList();

                if (controlsToDelete.Count > 0)
                {
                    var yPos = controlsToDelete.First().Location.Y;
                    var rowControls = panel3.Controls.Cast<Control>()
                                        .Where(c => c.Location.Y == yPos)
                                        .ToList();

                    foreach (var control in rowControls)
                    {
                        panel3.Controls.Remove(control);
                    }

                    AdjustControlPosition(); // Reposition remaining controls
                }
                else
                {
                    MessageBox.Show($"No TextBox found with text '{recordsNumber}'.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while deleting from the panel: {ex.Message}");
            }
        }

        private void AdjustControlPosition()
        {
            int labelWidth = 175;
            int textBoxWidth = 175;
            int rowHeight = 40;
            int spacing = 3;
            int yPos = 0;

            foreach (var rowControls in panel3.Controls.Cast<Control>()
                                    .GroupBy(c => c.Location.Y)
                                    .OrderBy(g => g.Key))
            {
                foreach (var control in rowControls)
                {
                    control.Location = new Point(control.Location.X, yPos);
                }
                yPos += rowHeight + spacing;
            }
            LCyPos = yPos;
            panel3.AutoScroll = true;
            panel3.AutoScrollMinSize = new Size(panel3.AutoScrollMinSize.Width, yPos);
        }

        private void Rotate_btn_Click(object sender, EventArgs e)
        {
            System.Drawing.Image impic = pictureBox1.Image;
            impic.RotateFlip(RotateFlipType.Rotate270FlipNone); 
            pictureBox1.Image = impic;  
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}


            
        
        
        
        
        



















       

      