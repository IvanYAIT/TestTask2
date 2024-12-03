using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Task1
{
    public class Bootstrapper : MonoBehaviour
    {
        private static string URL = "https://rc-today.ru/UserFiles/Image/a3/fd/kartina_po_nomeram_molly_prischepa_kraski_vechera_40h50_24_tsveta_kh0617_5eb2a3691b37a_6798_big.jpg";
        private static string SPRITE_NAME = "Image";
        private static string SCENE_NAME = "SampleScene";

        [SerializeField] private SceneView view;

        private void Start()
        {
            LoadWebImage(URL);
            LoadResourceImage(SPRITE_NAME);
            LoadScene(SCENE_NAME);
        }

        private async UniTask LoadWebImage(string url)
        {
            var uwr = (await UnityWebRequestTexture.GetTexture(url).SendWebRequest().ToUniTask(Progress.Create<float>(x => view.SetWebProgress(x))));
            if (uwr.result != UnityWebRequest.Result.Success)
                return;

            var texture = DownloadHandlerTexture.GetContent(uwr);
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            view.SetWebImage(sprite);
        }

        private async UniTask LoadResourceImage(string spriteName)
        {
            var sprite = await Resources.LoadAsync<Sprite>(spriteName).ToUniTask(Progress.Create<float>(x => view.SetResourceProgress(x)));

            view.SetResourceImage((Sprite) sprite);
        }

        private async UniTask LoadScene(string sceneName)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            await operation.ToUniTask(Progress.Create<float>( x => { 
                if(x == 0.9f)
                {
                    x = 1;
                    view.SceneBtn.interactable = true;
                    view.SceneBtn.onClick.AddListener(()=> operation.allowSceneActivation=true);
                }
                view.SetSceneProgress(x);
            }));
        }
    }
}