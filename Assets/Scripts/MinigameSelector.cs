using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameSelector : MonoBehaviour
{
    private List<Channel> activeGames = new List<Channel>();
    public List<Channel> minigamePossibilities;

    private List<Channel> usedGames = new List<Channel>();
    private List<Channel> availableGames = new List<Channel>();

    [HideInInspector]
    public List<Channel> activeChannels = new List<Channel>();

    public Channel staticChannel;

    [Tooltip("This should match the order of the suit enum")]
    public List<Sprite> suitSprites;

    private void Awake()
    {
        foreach (Channel chan in minigamePossibilities) {
            availableGames.Add(chan);
        }
        ResetChannels();
        PasscodeManager.NewPasscodeSet += NewPasscodeSet;
    }


    public void ResetChannels()
    {
        foreach (GameObject go in GameManager.Instance.channelPrefabs) {
            activeChannels.Add(go.GetComponent<Channel>());
        }
    }

    void NewPasscodeSet(string str)
    {
        SelectMiniGames();
    }

    public void SelectMiniGames()
    {
        ResetChannels();
        if (activeGames.Count == 0) {
            for (int i = 0; i < 4; i++) {
                PickOneGame();
            }
        } else {
            if (availableGames.Count == 0) {
                foreach (Channel chan in usedGames) {
                    availableGames.Add(chan);
                }
                usedGames.Clear();
            }
            Channel removedChan = activeGames[Random.Range(0, activeGames.Count)];
            activeGames.Remove(removedChan);
            usedGames.Add(removedChan);
            PickOneGame();
        }
        List<int> nums = new List<int>();
        for (int i = 0; i < 4; i++) {
            nums.Add(i);
        }
        foreach (Channel chan in activeGames) {
            int random = Random.Range(0, nums.Count);
            chan.SetSuit(suitSprites[nums[random]], nums[random]);
            nums.RemoveAt(random);
        }
        for (int i = activeChannels.Count - 1; i > 0; i--) {
            if (minigamePossibilities.Contains(activeChannels[i]) && !activeGames.Contains(activeChannels[i])) {
                activeChannels.RemoveAt(i);
            }
        }
    }

    void PickOneGame()
    {
        Channel selectedChan = availableGames[Random.Range(0, availableGames.Count)];
        activeGames.Add(selectedChan);
        availableGames.Remove(selectedChan);
    }

    public bool ActiveChannel(Channel channel)
    {
        if (minigamePossibilities.Contains(channel) && !activeGames.Contains(channel)) {
            staticChannel.ChannelEntered?.Invoke();
            return false;
        }
        staticChannel.ChannelExited?.Invoke();
        return true;
    }

    private void OnDestroy()
    {
        PasscodeManager.NewPasscodeSet -= NewPasscodeSet;
    }
}
