using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 

namespace LimoKnight
{

    public class DialogueManager : Singleton<DialogueManager>
    {
        [SerializeField] private GameObject _UICharacterDialogueObject;
        [SerializeField] private GameObject _imageTalkerObject;
        [SerializeField] private GameObject _textDialogueObject;
        [SerializeField] private GameObject _talkerNameObject;
        [SerializeField] private GameObject _nextDialogueIcon;
        [SerializeField] private float _dialogueSpeed = 1f;

        private Dictionary<int, Dialogue> _dialogueShow = new Dictionary<int, Dialogue>();
        private List<int> _freeIndexers = new List<int>();
        private Text _textDialogue;
        private Text _talkerName;
        private Image _imageTalker;

        private int _dialogueIndex;
        private int _dialogueNodeIndex = 0;
        private DialogueNode _actualNode;
        private Dialogue _actualDialogue;

        private bool _isLastNode= false;
        private bool _isDisplayingDialogue = false;
        private bool _isActualDialogueNodeEnded = false;

        private CharacterDialogueFade _dialogueGameObjectFade;


        public bool IsDisplayingDialogue
        {
            get { return _isDisplayingDialogue; }
        }

        public bool IsLastNode
        {
            get { return _isLastNode; }
        }

        public bool IsActualDialogueEnded
        {
            get { return _isActualDialogueNodeEnded; }
        }

        private void Start()
        {
            _textDialogue = _textDialogueObject.GetComponent<Text>();
            _talkerName = _talkerNameObject.GetComponent<Text>();
            _imageTalker = _imageTalkerObject.GetComponent<Image>();
            _dialogueGameObjectFade = _UICharacterDialogueObject.GetComponent<CharacterDialogueFade>();

        }

      /**  private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && _isDisplayingDialogue==false)
            {
                Dialogue dialogoPrueba = new Dialogue();
                DialogueNode dialogueNode = new DialogueNode();
                dialogueNode.SetValues("Tyler Joseph", "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789..", "/Inventory1", null);
                dialogoPrueba.AddDialogueNode(dialogueNode);
                DialogueNode dialogueNode2 = new DialogueNode();
                dialogueNode2.SetValues("Tyler Joseph", "This beat is a chemical, beat is a chemical", "/Inventory1", null);
                dialogoPrueba.AddDialogueNode(dialogueNode2);
                int dialogueToShow = AddDialogue(dialogoPrueba);
        
                ShowDialogue(dialogueToShow);
            }
            
            
            else if (Input.GetKeyDown(KeyCode.A))
            {
                InteractDialogue();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                AccelerateSpeed();
            }
           
        }*/

        //Method to the player when pressed the key (whatever, specified by the input) when
        //this class is called will use this and only this method.
        public void InteractDialogue()
        {
            if ((_isDisplayingDialogue == true) && (_isActualDialogueNodeEnded == true) && (!_isLastNode))
                DisplayNextDialogNode();
            else if (_isLastNode)
            {
                _dialogueGameObjectFade.HideDialogue();
                StartCoroutine("EndDialogue");
            }
        }


        private void AccelerateSpeed()
        {
            _dialogueSpeed = Mathf.Clamp01(_dialogueSpeed - _dialogueSpeed / 2f);
        }


        public bool ShowDialogue(int index)
        {
            if (_isDisplayingDialogue) return false;
            _dialogueIndex = index;
            DisplayDialogue();
            //after displaying it, we will remove it
            _dialogueShow.Remove(index);        
            _freeIndexers.Add(index);
            return true;
        }

        public int AddDialogue(Dialogue dialogue)
        {
            int index = -1;
            if (_freeIndexers.Count == 0)
                index = _dialogueShow.Count;
            else
            {
                index = _freeIndexers[0];
                _freeIndexers.RemoveAt(0);
            }
            _dialogueShow.Add(index, dialogue);
            return index;
        }


        private void DisplayDialogue()
        {
            _isDisplayingDialogue = true;
            _isActualDialogueNodeEnded = false;
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.CharacterDialogue);
            _actualDialogue = _dialogueShow[_dialogueIndex];
            DisplayDialogueNode(_actualDialogue.DialogueNodes[_dialogueNodeIndex]);
        }

        private void DisplayNextDialogNode()
        {
            _isActualDialogueNodeEnded = false;
            UIManager.Instance.ShowUIMainPanel(UIMainPanel.CharacterDialogue);
            DisplayDialogueNode(_actualDialogue.DialogueNodes[_dialogueNodeIndex]);
        }


        private void DisplayDialogueNode(DialogueNode node)
        {
            _textDialogue.text = "";
            _talkerName.text = "";
            _actualNode = node;
            _talkerName.text = node.NameTalker + ":";
            _imageTalker.sprite = Resources.Load("Sprites" + node.ImagePath, typeof(Sprite)) as Sprite;
            StartCoroutine(FadeText());
        }

        private IEnumerator FadeText()
        {
            //We shall not start fading the text until the maincanvas is active
            yield return new WaitUntil(() => _dialogueGameObjectFade.IsVisible == true);
            int numLetter = 0;
            string textShowed = "";
            while (numLetter < _actualNode.Text.Length)
            {
                yield return new WaitForSeconds(Time.deltaTime * _dialogueSpeed);
                textShowed = textShowed + _actualNode.Text[numLetter];
                _textDialogue.text = textShowed;
                numLetter++;
            }
            if (_dialogueNodeIndex != _actualDialogue.DialogueNodes.Count-1)
            {
                _isActualDialogueNodeEnded = true;
                _dialogueNodeIndex++;
            }
            else
            {
                _isLastNode = true;
                _dialogueNodeIndex = 0;
            }
            _dialogueSpeed = 1f;

            yield return null;
        }

        private IEnumerator EndDialogue()
        {
            yield return new WaitUntil(() => _dialogueGameObjectFade.IsNotVisible);
            _isDisplayingDialogue = false;      //in the end we're not displaying anydialogue (we're getting rid of it=
            _isLastNode = false;                //so the new one is not neither the lastone (because there's no one selected)
            _isActualDialogueNodeEnded = false;     //and neither is ended because it's not started
            UIManager.Instance.HideUIMainPanel(UIMainPanel.CharacterDialogue);
            yield return null;
        }
    }

    public class DialogueNode
    {
        private string _nameTalker;
        private string _text;
        private string _imagePath; //Path from Assets to SpriteIcon of npc talking starting with /
        private string _optionDialogue; //when the dialogue is done maybe it's need to do something called by (invoke(string))

        public string NameTalker
        {
            get { return _nameTalker; }
            set { _nameTalker = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        public string OptionsDialogue
        {
            get { return _optionDialogue; }
            set { _optionDialogue = value; }
        }

        public void SetValues(string nameTalker, string text, string ImagePath, string optionDialogue)
        {
            this.NameTalker = nameTalker; this.Text = text; this.ImagePath = ImagePath; this.OptionsDialogue = optionDialogue;
        }
    }
    public class Dialogue
    {

        private int _maxLengthDisplay = 200;
        private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();

        public List<DialogueNode> DialogueNodes
        {
            get { return _dialogueNodes; }
            set { _dialogueNodes = value; }
        }

        public void AddDialogueNode(DialogueNode node)
        {
            if (node.Text.Length >= _maxLengthDisplay)
            {
               for (int i=0; i < node.Text.Length;)
                {
                    DialogueNode dn = new DialogueNode();
                    dn.SetValues(node.NameTalker, "", node.ImagePath, "");
                    if (node.Text.Length > i)
                    {
                        dn.Text = node.Text.Substring(i, Mathf.Min(_maxLengthDisplay, node.Text.Length - i));
                        i = i + Mathf.Min(_maxLengthDisplay, node.Text.Length - i);
                    }
                    if (i == node.Text.Length)
                    {
                        dn.OptionsDialogue = node.OptionsDialogue;
                    }
                    _dialogueNodes.Add(dn);
                }
                return;
            }
            _dialogueNodes.Add(node);
        }
    }
}