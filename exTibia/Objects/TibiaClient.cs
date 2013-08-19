using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Diagnostics;

using exTibia.Helpers;
using exTibia.Modules;

namespace exTibia.Objects
{
    public class TibiaClient
    {
        #region Fields

        private Process _process;
        private string _title;

        #endregion

        #region Properties

        public Process Process
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return _process; }
            set { _process = value; }
        }

        public string Title
        {
            [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
            get { return _title; }
            set { _title = value; }
        }

        #endregion

        #region Constructor

        [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        public TibiaClient(Process process)
        {
            this.Process = process;
            GameClient gameClient = new GameClient(this);
            gameClient.AssignProcess();
            this.Title = string.Format("#{0} {1}",Process.Id,TibiaClients.GetCharacterNameFromProcess(Process));
        }

        #endregion
    }
}
