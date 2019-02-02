using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public class NPCTalk : NPC
    {
        [SerializeField] protected string _name;

        [Tooltip("Es el nombre del sprite que tenga en la carpeta /Sprites")]
        [SerializeField] protected string _spritePath;
        [TextArea] [SerializeField] protected string[] _dialogues;
        private int i = 0;

        protected virtual void Update()
        {
            if ((this._inside) && Input.GetKeyUp(InputManager.Instance.KeyBindings.Action) && DialogueManager.Instance.IsDisplayingDialogue == false)
            {
                i = i % this._dialogues.Length;
                Dialogue d = new Dialogue();
                DialogueNode dn = new DialogueNode();
                dn.SetValues(this._name, this._dialogues[i], _spritePath, null);
                d.AddDialogueNode(dn);
                int dialogueToShow = DialogueManager.Instance.AddDialogue(d);
                DialogueManager.Instance.ShowDialogue(dialogueToShow);
                i++;
            }
           if (Input.GetKeyUp(InputManager.Instance.KeyBindings.Action)&& DialogueManager.Instance.IsDisplayingDialogue == true)
                DialogueManager.Instance.InteractDialogue();
        }

        protected override void CompleteQuest()
        {
            base.CompleteQuest();
        }
    }
}


