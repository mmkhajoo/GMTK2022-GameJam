using System;
using System.IO;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Playables;

public class CharacterVisual : MonoBehaviour
{
   [SerializeField] private Animator animator;
   
   [SerializeField] private GameObject characterSpriteGameObject;
   [SerializeField] private GameObject trailGameObject;
   
   [SerializeField] private GameObject jumpParticle;
   [SerializeField] private GameObject moveParticle;
   [SerializeField] private GameObject landParticle;
   [SerializeField] private GameObject dashParticle;
   [SerializeField] private GameObject dieParticle;
   
   
   private static readonly int Speed = Animator.StringToHash("Speed");
   private static readonly int Jump = Animator.StringToHash("Jump");
   private static readonly int Walk = Animator.StringToHash("Walk");
   private static readonly int Idle = Animator.StringToHash("Idle");
   private static readonly int Land = Animator.StringToHash("Land");
   // private static readonly int Dash = Animator.StringToHash("Land");
   
   private void Start()
   {
      jumpParticle.SetActive(false);
      moveParticle.SetActive(true);
      landParticle.SetActive(false);
      dashParticle.SetActive(false);
      dieParticle.SetActive(false);
   }

   private void Update()
   {
      var speedX=Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x);
      animator.SetFloat(Speed,speedX);
   }

   public void OnPlayerStateChange(PlayerStateType state)
   {
      switch (state)
      {
         case PlayerStateType.None:
          //  DoIdle();
            break;
         case PlayerStateType.Idle:
            DoIdle();
            break;
         case PlayerStateType.Walking:
            DoWalk();
            break;
         case PlayerStateType.Die:
            DoDie();
            break;
         default:
            //DoIdle();
            break;
      }
   }

   public void OnPlayerJump()
   {
      DoJump();
   }
   
   public void OnPlayerLand()
   {
      DoLand();
   }

   public void OnPlayerDash()
   {
      DoDash();
   }
   
   private void DoWalk()
   {
      moveParticle.SetActive(true);
      animator.SetTrigger(Walk);
   }
   private void DoJump()
   {
      animator.ResetTrigger(Land);
      animator.SetTrigger(Jump);
      
      moveParticle.SetActive(false);
      
      jumpParticle.SetActive(false);
      jumpParticle.SetActive(true);
   }
   private void DoIdle()
   {
      moveParticle.SetActive(true);
      animator.SetTrigger(Idle);
   }
   private void DoLand()
   {
      animator.SetTrigger(Land);
      
      landParticle.SetActive(false);
      landParticle.SetActive(true);
   }
   
   private void DoDie()
   {
      characterSpriteGameObject.SetActive(false);
      
      dieParticle.SetActive(false);
      dieParticle.SetActive(true);
      
      characterSpriteGameObject.SetActive(false);
      trailGameObject.SetActive(false);
   }
   
   private void DoDash()
   {
      dashParticle.SetActive(false);
      dashParticle.SetActive(true);
   }
}
