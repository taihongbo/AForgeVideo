using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace AForgeVideo
{
    public partial class VideoMain : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoDevice;
        private VideoCapabilities[] videoCapabilities;
        private int videoResolutionsIndex;
        public VideoMain()
        {
            InitializeComponent();
        }

        private void VideoMain_Load(object sender, EventArgs e)
        {
            InitializeCamera();
        }

        private void InitializeCamera() {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice); 
            if (videoDevices.Count != 0)
            { 
                foreach (FilterInfo device in videoDevices)
                {
                    devicesCombo.Items.Add(device.Name);
                }
                devicesCombo.SelectedIndex = 0;
                videoResolutionsIndex = 0;
            }
            else
            {
                devicesCombo.Items.Add("没有找到可用的摄像头");
                videoResolutionsIndex = -1;
            } 
        }
        private void EnableConnectionControls(bool enable)
        {
            devicesCombo.Enabled = enable; 
            connectButton.Enabled = enable;
            disconnectButton.Enabled = !enable;
            SettingButton.Enabled = !enable;
        }

        private void ConnectVideo()
        {
            if (videoDevice != null)
            {
                if ((videoCapabilities != null) && (videoCapabilities.Length != 0))
                {
                    videoDevice.VideoResolution = videoCapabilities[videoResolutionsIndex];
                }  
                EnableConnectionControls(false); 
                videoSourcePlayer.VideoSource = videoDevice;
                videoSourcePlayer.Start();
            }
        }


        private void DisconnectVideo()
        {
            if (videoSourcePlayer.VideoSource != null)
            {
                // stop video device
                videoSourcePlayer.SignalToStop();
                videoSourcePlayer.WaitForStop();
                videoSourcePlayer.VideoSource = null; 
                EnableConnectionControls(true);
            }
        }

        private void devicesCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDevices.Count != 0)
            {
                videoDevice = new VideoCaptureDevice(videoDevices[devicesCombo.SelectedIndex].MonikerString);
                EnumeratedSupportedFrameSizes(videoDevice);
            }
        }

        private void EnumeratedSupportedFrameSizes(VideoCaptureDevice videoDevice)
        {
            this.Cursor = Cursors.WaitCursor; 
            try
            {
                int i = 0;
                videoCapabilities = videoDevice.VideoCapabilities;
                foreach (VideoCapabilities capabilty in videoCapabilities)
                {
                    if (capabilty.FrameSize.Width == 640 && capabilty.FrameSize.Height == 480) {
                        videoResolutionsIndex = i;
                    }
                    i = i + 1;
                }  
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            ConnectVideo();
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            DisconnectVideo();
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            videoDevice.DisplayPropertyPage(IntPtr.Zero); 
           
        }

        private void VideoMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectVideo();
        }

        private void videoSourcePlayer_Paint(object sender, PaintEventArgs e)
        {
       
        }
    }
}
