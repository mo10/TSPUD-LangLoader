using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entrypoint
{
    internal class HardcodedTextPatch
    {

        public static void DoPatch(GameObject gameObject)
        {
            // Scene Load
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.Debug($"Try patching textes in scene {scene.name}");
            if (scene.name == "Settings_UD_MASTER")
            {
                // Patch Game UI Text
                var gm = GameObject.Find("_TSPUD_MENU(Clone)");
                if (gm == null)
                {
                    Logger.Error("Can not find _TSPUD_MENU");
                    return;
                }

                var textes = gm.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
                foreach (var t in textes)
                {
                    if (t.text == null) continue;
                    Logger.Debug($"Patching {t.name}, text: {t.text}");
                    switch (t.name)
                    {
                        case "Header_Text_Lower":
                            t.text = t.text.Replace("THE STANLEY PARABLE", "<line-height=110%>史丹利的寓言").
                                Replace("ULTRA DELUXE", "究极豪华版");
                            break;
                        case "TSP_1_Header":
                        case "Header_Text_Title":
                            t.text = t.text.Replace("THE STANLEY PARABLE", "<voffset=0.15em>史丹利的寓言<voffset=0>");
                            Logger.Debug($"Patched text: ${t.text}");
                            break;
                        case "Header_Text":
                            if (t.text == "The sequel is now paused")
                            {
                                t.text = "续作已暂停";
                                t.name = $"Patched {t.name}";
                            }
                            break;
                    }
                }

                // Patch subtitle setting
                gm = GameObject.Find("Settings_Character_Button (1)");
                if (gm == null)
                {
                    Logger.Error("cannot find Settings_Character_Button (1)");
                }

                var text = gm.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = "简体中文";
                }
                
            }
            else if (scene.name == "LoadingScene_UD_MASTER")
            {
                // loading scene 
                var gm = GameObject.Find("All Elements");
                var uitextes = gm.GetComponentsInChildren<UnityEngine.UI.Text>();
                foreach (var t in uitextes)
                {
                    Logger.Debug($"Patching {t.name}, text: {t.text}, scene: {scene.name}");
                    switch (t.name)
                    {
                        case "Text Left":
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.fontStyle = FontStyle.Bold;
                            t.text = "结束永不结束永不结束永不结束永不结束永不结束永不结束永不结束永不";
                            t.name = $"Pathced {t.name}";
                            break;
                        case "Text Right":
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.fontStyle = FontStyle.Bold;
                            t.text = "结束永不结束";
                            t.name = $"Pathced {t.name}";
                            break;
                        case "LOADING":
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.fontStyle = FontStyle.Bold;
                            t.text = "加载中";
                            t.name = $"Pathced {t.name}";
                            break;
                    }
                }

                var tmtextes = gm.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
                foreach (var t in tmtextes)
                {
                    Logger.Debug($"Patching {t.name}, text: {t.text}, scene: {scene.name}");
                    switch (t.name)
                    {
                        case "TextMeshPro Text":
                            if (t.text == "Practice") t.text = "练习";
                            break;
                    }
                    Logger.Debug($"Patched text: {t.text}");
                }
            }
            else if (scene.name == "theonlymap_UD_MASTER")
            {
                // Patch Adventure Line Text
                var gm = GameObject.Find("Adventure Line TMP");
                if (gm == null)
                {
                    Logger.Error("Can not find Adventure Line TMP");
                    return;
                }

                var textes = gm.GetComponentsInChildren<TMPro.TextMeshPro>();
                foreach (var t in textes)
                {
                    if (t.text == null) continue;
                    Logger.Debug($"Patching {t.name}, text: {t.text}");
                    switch (t.name)
                    {
                        case "Text TMP The":
                            t.text = "";
                            break;
                        case "Text TMP Stanley Parable":
                            t.text = "史丹利的寓言";
                            Logger.Debug($"Patched text: ${t.text}");
                            break;
                        case "Text TMP Stanley Parable 2":
                            t.text = "史丹利的寓言2";
                            Logger.Debug($"Patched text: ${t.text}");
                            break;
                        case "Text TMP Adventure Line":
                            t.text = "冒险专线";
                            Logger.Debug($"Patched text: ${t.text}");
                            break;
                    }
                }
            }
            else if (scene.name == "buttonworld_UD_MASTER")
            {
                var gm = GameObject.Find("text");
                if (gm == null)
                {
                    Logger.Error("Can not find text");
                }

                var textes = gm.GetComponentsInChildren<UnityEngine.UI.Text>();
                foreach (var t in textes)
                {
                    Logger.Debug($"Patching {t.name}, text: {t.text}, scene: {scene.name}");
                    switch (t.name)
                    {
                        case "Welcome Stanley, to heaven.":
                        case "Welcome Stanley, to heaven.(Clone)":
                            t.text = "欢迎，史丹利，来到天堂。";
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.name = $"Patched {t.name}";
                            break;
                    }
                }

            }
            else if (scene.name == "thefirstmap_UD_MASTER")
            {
                var gm = GameObject.Find("youwin");
                if (gm == null)
                {
                    Logger.Error("Can not find Adventure Line TMP");
                }

                var textes = gm.GetComponentsInChildren<UnityEngine.UI.Text>();
                foreach (var t in textes)
                {
                    Logger.Debug($"Patching {t.name}, text: {t.text}, scene: {scene.name}");
                    switch (t.name)
                    {
                        case "YOU WIN!!":
                        case "YOU WIN!! Shadow":
                            t.text = "你赢了!!";
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.name = $"Patched {t.name}";
                            break;
                    }
                }
            }
            else if (scene.name == "incorrect_UD_MASTER")
            {
                Logger.Debug("Going find textes in incorrect_UD_MASTER");
                var gm = GameObject.Find("Credits Canvas");
                if (gm == null)
                {
                    Logger.Debug("cannot find Credits Canvas");
                    Debug.Log("cannot find Credits Canvas");
                    return;
                }
                var textes = gm.GetComponentsInChildren<UnityEngine.UI.Text>(true);
                Logger.Debug($"get text count {textes.Length}");
                foreach(var t in textes)
                {
                    Logger.Debug($"Patching {t.name}, text: {t.text}, scene: {scene.name}");
                    switch (t.name)
                    {
                        case "The End Text":
                            t.text = "全剧终!";
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.name = $"Patched {t.name}";
                            break;
                        case "Thank you for Playing Text":
                            t.text = "史丹利的寓言:\n究极豪华版";
                            t.fontSize = (int)(t.fontSize * 0.726);
                            t.name = $"Patched {t.name}";
                            break;
                    }
                }
            }
        }
    }
}
