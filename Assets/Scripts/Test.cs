using Babble;
using Babble.Commands;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Stage))]
public class Test : MonoBehaviour {
	
	[Serializable]
	public struct PuppetStruct {
		public string id;
		public TextAsset puppet;
	}

	public TextAsset script;
	public PuppetStruct[] puppets;
    public GameObject chatboxPrefab;

	private Stage stage;
	private Puppet puppet;
    private Cutscene cutscene;
    private bool control = false;
    private Dictionary<string, string> actors;

	void Awake() {
		stage = GetComponent<Stage>();
	}

	void Start () {
        actors = new Dictionary<string, string>();
        foreach (PuppetStruct puppetStruct in puppets) {
            actors.Add(puppetStruct.id, puppetStruct.puppet.text);
        }

        Cutscene cutscene = new Cutscene(stage, actors);
        ChatterCommandFactory factory = new ChatterCommandFactory();
        factory.chatboxPrefab = chatboxPrefab;
        cutscene.commandFactories.Add("chatter", factory);
        cutscene.Start(cutscene.ReadScript(script.text), delegate { control = true; puppet = stage.GetPuppet(1); });
	}
	
	void Update () {
        if (control) {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                puppet.MoveLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow))
                puppet.MoveRight();
            if (Input.GetKeyDown(KeyCode.UpArrow))
                puppet.Jiggle();
            if (Input.GetKeyDown(KeyCode.Space))
                puppet.SetBabbling(true);
            if (Input.GetKeyUp(KeyCode.Space))
                puppet.SetBabbling(false);
            if (Input.GetKeyDown(KeyCode.U))
                puppet.ChangeEmote(0);
            if (Input.GetKeyDown(KeyCode.I))
                puppet.ChangeEmote(1);
            if (Input.GetKeyDown(KeyCode.O))
                puppet.ChangeEmote(2);
            if (Input.GetKeyDown(KeyCode.P))
                puppet.ChangeEmote(3);
            if (Input.GetKeyDown(KeyCode.J))
                puppet.ChangeEmote(4);
            if (Input.GetKeyDown(KeyCode.K))
                puppet.ChangeEmote(5);
            if (Input.GetKeyDown(KeyCode.L))
                puppet.ChangeEmote(6);
            if (Input.GetKeyDown(KeyCode.Semicolon))
                puppet.ChangeEmote(7);
            if (Input.GetKeyDown(KeyCode.M))
                puppet.ChangeEmote(8);
            if (Input.GetKeyDown(KeyCode.Comma))
                puppet.ChangeEmote(9);
            if (Input.GetKeyDown(KeyCode.Period))
                puppet.ChangeEmote(10);
            if (Input.GetKeyDown(KeyCode.Slash))
                puppet.ChangeEmote(11);
        }
    }
}
