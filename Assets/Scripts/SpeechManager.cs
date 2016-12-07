using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{

    public static SpeechManager Instance { get; private set; }

    Dictionary<string, System.Action> _keywords = new Dictionary<string, System.Action>();

    private KeywordRecognizer _keywordRecognizer;

    void Awake()
    {
        Instance = this;

        _keywords.Add("Create cannon", GazeGestureManager.Instance.CreateCannon);
        _keywords.Add("Activate moving", GazeGestureManager.Instance.ActivateMoveObjectsMode);
        _keywords.Add("Activate aiming", GazeGestureManager.Instance.ActivateRotateObjectsMode);
        _keywords.Add("Activate firing", GazeGestureManager.Instance.ActivateFiringMode);

        _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;

        if (_keywords.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke();
        }
    }
    
}
