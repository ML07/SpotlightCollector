/* -------------------------------------------------------------------------------------------------
   Restricted - Copyright (C) Siemens Healthcare GmbH/Siemens Medical Solutions USA, Inc., 2019. All rights reserved
   ------------------------------------------------------------------------------------------------- */

namespace SL.ViewModels
{
    class MainViewModel
    {
        public MainViewModel()
        {
            LogsToUI = new FileCopy().StartFileCopy();
        }
        public string LogsToUI { get; set; }
    }
}
