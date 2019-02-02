using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LimoKnight
{
    public enum SlotMachineMode
    {
        None, 
        Simple,
        Double, 
        Triple,
        Quadruple,
    }
    public class SlotMachine : MonoBehaviour 
    {
        //Add'em in order: left-middle-right
        [SerializeField] private RectTransform[] SlotsRows;
        [SerializeField] private GameObject _exitButton;
        [SerializeField] private GameObject _stopButton;
        private ItemDroper ItemDroper = new ItemDroper();
        private SlotMachineMode _slotMode = SlotMachineMode.None;
        private bool _endedRotation = true;
        private bool _waitUntilTrueEnd = true;
        private float _maxMoveVelocityEachFrame = 40f;
        private float _actualVelocityLeft = 0f;
        private float _actualVelocityRight = 0f;
        private float _actualVelocityCenter = 0f;
        private float _limitInferior = -570f;
        private float _limitSuperior = 600f;

        private float _distanceBetweenSlots = 200f;
        private float _superiorSlotPosition = 400f;
        private float _inferiorSlotPosition = -400f;
        private bool _pressedEnded = false;
        [SerializeField] private int _slotsEnded = 0;
        private int _needingAdjusting = 0;
        

        public void SetSlotMachineMode(int rotateMode)
        {
            if (_endedRotation && _slotMode == SlotMachineMode.None)
            {
                int cost = 0;
                if ((SlotMachineMode)rotateMode == SlotMachineMode.Simple) cost = 50;
                else if ((SlotMachineMode)rotateMode == SlotMachineMode.Double) cost = 100;
                else if ((SlotMachineMode)rotateMode== SlotMachineMode.Triple) cost = 150;
                else if ((SlotMachineMode)rotateMode== SlotMachineMode.Quadruple) cost = 200;
                if (StatsManager.Instance.InfoPlayer.ActualGold >= cost)
                {
                    StatsManager.Instance.AddGold(-cost);
                    _slotMode = (SlotMachineMode)rotateMode;
                    StartCoroutine(StartRotatingMode());
                    _exitButton.SetActive(false);
                }
            }
        }

        public void StopButton()
        {
            StartCoroutine(StopSlotsCoroutine());
        }

        public void ExitSlot()
        {
            GameManager.Instance.SwapContextToWorldFromMinigame();
        }

        private IEnumerator StartRotatingMode()
        {

            _actualVelocityLeft = Time.deltaTime +  _actualVelocityLeft;
            _actualVelocityCenter = Time.deltaTime + _actualVelocityCenter;
            _actualVelocityRight = Time.deltaTime + _actualVelocityRight;

            StartCoroutine(MoveSlotsRowCoroutine(SlotsRows[0], "Left"));
            StartCoroutine(MoveSlotsRowCoroutine(SlotsRows[1], "Center"));
            StartCoroutine(MoveSlotsRowCoroutine(SlotsRows[2], "Right"));
            yield return null;
        }

        private IEnumerator MoveSlotsRowCoroutine(RectTransform slotRow, string slot)
        {
            float velocity = 0;
            velocity = velocity  + Time.deltaTime * Random.Range(3f, 5f);
            while(_slotMode != SlotMachineMode.None && velocity > 0 && !_pressedEnded)
            {
                if (velocity > _maxMoveVelocityEachFrame)
                    velocity = _maxMoveVelocityEachFrame;
                ///Move slot down _actualVelocity [position.x = position.x+_actualVeloc]
                slotRow.anchoredPosition = new Vector3(0f, slotRow.anchoredPosition.y + velocity, 0f);
                if (slotRow.anchoredPosition.y > _limitSuperior)
                    slotRow.anchoredPosition = new Vector3(0f, _limitInferior, 0f);
                if (slot == "Left")
                    _actualVelocityLeft = velocity;
                else if (slot == "Center")
                    _actualVelocityCenter = velocity;
                else if (slot == "Right")
                    _actualVelocityRight = velocity;
                yield return new WaitForEndOfFrame();
                velocity = velocity + Time.deltaTime * Random.Range(3f, 10f);
            }
            yield return null;
        }

        public IEnumerator StopSlotsCoroutine()
        {
            if (_slotMode != SlotMachineMode.None && _actualVelocityRight == _actualVelocityLeft &&
                _actualVelocityLeft == _actualVelocityRight && _actualVelocityRight == _maxMoveVelocityEachFrame)
            {
                _pressedEnded = true;
                StartCoroutine(StopSlotsRowCoroutine(SlotsRows[0], "Left"));
                StartCoroutine(StopSlotsRowCoroutine(SlotsRows[1], "Center"));
                StartCoroutine(StopSlotsRowCoroutine(SlotsRows[2], "Right"));
                yield return new WaitUntil(() => _slotsEnded == 3);
                _actualVelocityCenter = 0f;
                _actualVelocityLeft = 0f;
                _actualVelocityRight = 0f;
                StartCoroutine(AdjustPositions());
            }
        }

        private IEnumerator StopSlotsRowCoroutine(RectTransform slotRow, string slot)
        {

            float velocity = -1f;
            if (slot == "Left")
                velocity = _actualVelocityLeft;
            else if (slot == "Center")
                velocity = _actualVelocityCenter;
            else if (slot == "Right")
                velocity = _actualVelocityRight;
            velocity = velocity * Random.Range(2f, 6f) - Time.deltaTime * Random.Range(1f, 6f);
            while (_slotMode != SlotMachineMode.None && velocity > 0)
            {
                if (velocity > _maxMoveVelocityEachFrame)
                    velocity = _maxMoveVelocityEachFrame;
                ///Move slot down _actualVelocity [position.x = position.x+_actualVeloc]
                slotRow.anchoredPosition = new Vector3(0f, slotRow.anchoredPosition.y + velocity, 0f);
                if (slotRow.anchoredPosition.y > _limitSuperior)
                    slotRow.anchoredPosition = new Vector3(0f, _limitInferior, 0f);
               
                yield return new WaitForEndOfFrame();
                velocity = velocity - Time.deltaTime * Random.Range(1f, 20f);
            }
            _slotsEnded++;
            yield return null;
        }

        private IEnumerator AdjustPositions()
        {
            float newY = 0f;
            foreach (RectTransform go in SlotsRows){
                if (go.anchoredPosition.y%200f != 0)
                {
                    int result = Mathf.RoundToInt(go.anchoredPosition.y / _distanceBetweenSlots); 
                    //201.2 -> 1 ó -200.1 || -> 1*200 ó -300.1 -> -> -2--> -400
                    newY = result * _distanceBetweenSlots;
                    if (newY > _superiorSlotPosition)
                        newY = _superiorSlotPosition;
                    else if (newY < _inferiorSlotPosition)
                        newY = _inferiorSlotPosition;
                    StartCoroutine(AdjustPositionsSlot(go, newY));
                    _needingAdjusting++;
                }
            }
            yield return new WaitUntil(() => _needingAdjusting == 0);
            GiveRewards();
            yield return null;
        }

        private IEnumerator AdjustPositionsSlot(RectTransform go, float positionToBe)
        {
            while(go.anchoredPosition.y != positionToBe)
            {
                float move = Time.deltaTime * Random.Range(40f, 70f);
                    if (positionToBe > go.anchoredPosition.y)
                    { //[400-->395] ó [-200 -- -299]
                        go.anchoredPosition = new Vector3(0f, go.anchoredPosition.y + move, 0f);
                        if (go.anchoredPosition.y > positionToBe)
                            go.anchoredPosition = new Vector3(0f, positionToBe, 0f);
                    }
                    else
                    {    //[200---295] ó [-400 -- -395f]
                        go.anchoredPosition = new Vector3(0f, go.anchoredPosition.y - move, 0f);
                        if (go.anchoredPosition.y < positionToBe)
                            go.anchoredPosition = new Vector3(0f, positionToBe, 0f);
                    }
                yield return new WaitForEndOfFrame();
            }
            _needingAdjusting--;
            yield return null;
        }

        private void GiveRewards()
        {
            //because the level of the player is used to give the rarity of the item, we will use how many
            //gold you use to increase this factor and give more items
            float divisorLevel = (5f - (int)_slotMode)/2f;
           if (SlotsRows[0].anchoredPosition.y == SlotsRows[1].anchoredPosition.y &&
                SlotsRows[0].anchoredPosition.y == SlotsRows[2].anchoredPosition.y)
           {
                if (SlotsRows[0].anchoredPosition.y == 0f) //GIVE GOLD!
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel/divisorLevel), TypeDrop.Gold);
                else if (SlotsRows[0].anchoredPosition.y == 200f) //Give Armor
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Armor);
                else if (SlotsRows[0].anchoredPosition.y == 400f) //Give Item
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Item);
                else if (SlotsRows[0].anchoredPosition.y == -200f) //Give Weapon
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Weapon);
                 else if (SlotsRows[0].anchoredPosition.y == -400f) //¡GIVE ALL!
                {
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Gold);
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Armor);
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Item);
                    ItemDroper.DropItemDirectly(Mathf.RoundToInt(StatsManager.Instance.InfoPlayer.ActualLevel / divisorLevel), TypeDrop.Weapon);
                }
           }
            RestoreSlot();
        }

        private void RestoreSlot()
        {
            _actualVelocityLeft = 0f;
            _actualVelocityRight = 0f;
            _actualVelocityCenter = 0f;
            _endedRotation = true;
            _slotMode = SlotMachineMode.None;
            _exitButton.SetActive(true);
            _pressedEnded = false;
            _slotsEnded = 0;
             _needingAdjusting = 0;
        }
    }
}