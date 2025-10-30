using UnityEngine;
using EZCameraShake;

public class MeleeTool : Tool
{
    private bool swingingThisFrame;
    private bool usedDuringSwing;
    private bool checkAfterFrame;
    private readonly float swingTrigger = 0.2f;

    [SerializeField] private AudioClip swingAudio;

    private ToolController toolController;

    private void Start()
    {
        toolController = ToolController.Instance;
    }

    public override void HandleInput()
    {
        if (toolController == null)
            toolController = ToolController.Instance;

        bool isSwinging = Input.GetMouseButton(0);

        if (isSwinging && toolController.useTimer <= 0 && !swingingThisFrame)
        {
            float baseLength = 1f;
            float speedMultiplier = baseLength / data.cooldown;

            toolAnimator.speed = speedMultiplier;
            toolAnimator.Play("Swing", 0, 0f);
            AudioManager.Play(swingAudio, Vector3.zero, 0.8f, 1.2f, 0.3f, false);

            usedDuringSwing = false;
            checkAfterFrame = false;
            toolController.useTimer = data.cooldown;
            swingingThisFrame = true;
        }

        swingingThisFrame = false;
    }

    public override void UpdateTool()
    {
        if (checkAfterFrame && !usedDuringSwing)
        {
            var info = toolAnimator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName("Swing") && info.normalizedTime >= swingTrigger)
            {
                CameraShaker.Instance.ShakeOnce(2f, 2f, 0.15f, 0.2f);
                Use();
                usedDuringSwing = true;
            }
        }

        checkAfterFrame = true;
    }

    protected virtual void Use()
    {
        if (Physics.Raycast(PlayerCamera.GetRay(), out RaycastHit hit, 5f))
        {
            bool crit = Combat.RollCrit();

            IDamageable damageable = GetTarget(hit.collider.transform);

            if (damageable is BaseMineable mineable)
                PlayerCombat.DamageMineable(data.damage, crit, mineable, hit.point, hit.normal, data.type);
            else if (damageable is BaseEnemy enemy)
                PlayerCombat.DamageEnemy(data.damage, crit, enemy, hit.point, hit.normal);
        }
    }

    private static IDamageable GetTarget(Transform target)
    {
        while (target != null)
        {
            var m = target.GetComponent<IDamageable>();
            if (m != null)
                return m;

            target = target.parent;
        }

        return null;
    }
}