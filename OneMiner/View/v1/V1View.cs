﻿using OneMiner.Coins;
using OneMiner.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneMiner.View.v1
{
    class V1View: IView
    {
        MainForm m_MainForm = null;
        public TSQueue<DownloadRequest> DownloadRequestQueue { get; set; }

        System.Windows.Forms.Timer m_Timer;
        event OneMinerTimerEvent m_UIEvents;
        public void InitializeView()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DownloadRequestQueue = new TSQueue<DownloadRequest>();

            
            m_Timer = new System.Windows.Forms.Timer();
            RegisterForTimer(ExecuteDownloadRequests);
            m_Timer.Interval = 3000;
#if DEBUG
            m_Timer.Interval = 10000;
#endif
            m_Timer.Tick += t_Tick;
        }



        public void RegisterForTimer(OneMinerTimerEvent fun)
        {
            m_UIEvents += fun;
        }
        public void StartView()
        {

            m_MainForm = new MainForm();
            m_MainForm.DownloadBrowser.DocumentCompleted += DownloadBrowser_DocumentCompleted;
            UpdateMinerList();
            m_Timer.Start();
            Application.Run(m_MainForm);
        }
        void t_Tick(object sender, EventArgs e)
        {
            m_UIEvents.Invoke();
        }
        public void UpdateMinerList()
        {
            m_MainForm.UpdateMinerList();
        }
        public void UpDateMinerState()
        {
            m_MainForm.UpDateMinerState();
        }

        void DownloadBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            DownloadRequest currentRequest = null;
            try
            {
                HtmlElement eas = m_MainForm.DownloadBrowser.Document.Body;
                currentRequest = m_MainForm.DownloadBrowser.DownloadRequest;
                if (currentRequest != null)
                {
                    currentRequest.Reader.LastLog = eas.InnerText;
                    currentRequest.Reader.Parse();
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void ExecuteDownloadRequests()
        {
            try
            {
                DownloadRequest  currentRequest = DownloadRequestQueue.Dequeue();
                if (currentRequest != null)
                {
                    m_MainForm.DownloadBrowser.DownloadRequest = currentRequest;
                    m_MainForm.DownloadBrowser.Navigate(currentRequest.LINK);
                }
            }
            catch (Exception e)
            {
            }
      
        }
        
    }
    public class DownloadRequest
    {
        public string LINK { get; set; }
        public OutputReaderBase Reader{ get; set; }
    }
    public class WebBrowserEx: WebBrowser
    {
        public DownloadRequest DownloadRequest { get; set; }

         
    }
}
