using Misc;
using UI.ChatLog;
using UnityEngine;

namespace Environment {
    public class ShowMessageOnInteract : Interact
    {
        public string messageLink;
        public bool oneShot;

        private const float cooldown = 0.0f;

        private readonly ChatLog chatLog = AutowireFactory.GetInstanceOf<ChatLog>();
        private readonly Timer cooldownTimer = new Timer();
        
        public override void OnInteract() {
            if (cooldownTimer.IsRunning()) {
                return;
            }
            
            chatLog.Post(messageLink);
            cooldownTimer.Start(cooldown);
            if (oneShot) {
                DisableInteraction();
                Destroy(this);
            }
        }
    }
}
