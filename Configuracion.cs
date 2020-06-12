using Rocket.API;
using System.Collections.Generic;

namespace Darkness
{
    public class Configuracion : IRocketPluginConfiguration
    {
        public float LeaveRobSecondsForGoBack;

        public string IconMessages;

        public ushort UIReward;

        public string PolicePermission;

        public string startrobmessageicon;

        public string leavearearobmessageicon;

        public string finishrobmessageicon;

        public string iconwarnwithoutui;

        public string iconwarn;

        public bool UconomyOrXp;

        public void LoadDefaults()
        {
            UconomyOrXp = false;
            PolicePermission = "policia";
            UIReward = 16999;
            LeaveRobSecondsForGoBack = 1;
            startrobmessageicon = "https://lineex.es/wp-content/uploads/2018/06/alert-icon-red-11-1.png";
            finishrobmessageicon = "https://lineex.es/wp-content/uploads/2018/06/alert-icon-red-11-1.png";
            leavearearobmessageicon = "https://lineex.es/wp-content/uploads/2018/06/alert-icon-red-11-1.png";
            iconwarn = "https://tuc.vampyrium.com/warn/triangle_1024.png";
            iconwarnwithoutui = "https://tuc.vampyrium.com/warn/triangle_1024.png";
        }
    }
}
