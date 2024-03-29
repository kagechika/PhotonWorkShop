using UnityEngine;

public class NetworkCharacter : Photon.MonoBehaviour
{
    private Vector3 correctPlayerPos = Vector3.zero; // We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; // We lerp towards this
    
	void Awake()
	{
		if (photonView.isMine) {
			GetComponent<ThirdPersonCamera>().enabled = true;
			GetComponent<MonsterFire>().enabled = true;
		}
	}

	// Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            transform.position = Vector3.Lerp(transform.position, this.correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, this.correctPlayerRot, Time.deltaTime * 5);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		// send
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            myThirdPersonController myC = GetComponent<myThirdPersonController>();
            stream.SendNext((int)myC._characterState);
        }
		// get
        else
        {
            // Network player, receive data
            this.correctPlayerPos = (Vector3)stream.ReceiveNext();
            this.correctPlayerRot = (Quaternion)stream.ReceiveNext();

            myThirdPersonController myC = GetComponent<myThirdPersonController>();
            myC._characterState = (CharacterState)stream.ReceiveNext();
        }
    }

	[RPC]
	void Destroy()
	{
		PhotonNetwork.Destroy(gameObject);
	}
}
