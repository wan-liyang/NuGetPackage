﻿using MicrosoftGraphService.Config;
using MicrosoftGraphService.HttpClientHelper;
using MicrosoftGraphService.Model;
using System.Collections.Generic;
using System.Linq;

namespace MicrosoftGraphService.Helper
{
    public class MsGraphOutlookHelper
    {
        private static MailboxHelper _helper;
        public MsGraphOutlookHelper(MsGraphApiConfig config)
        {
            MsGraphHelper graphHelper = new MsGraphHelper(config);
            _helper = new MailboxHelper(graphHelper.GetHttpClient());
        }

        public List<MailboxModel.Message> GetMessages(string userEmail, string folder)
        {
            var messages = _helper.GetMessages(userEmail, folder).value;
            var list = messages.Select(p => p.ToMessageModule()).ToList();

            return list;
        }
    }
}
