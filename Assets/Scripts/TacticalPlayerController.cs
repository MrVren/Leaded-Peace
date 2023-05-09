using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

namespace LeadedPeace
{
    public class TacticalPlayerController : MonoBehaviour
    {
        [SerializeField] private Canvas _scopingCanvas;

        //[SerializeField] private CinemachineVirtualCamera _followVirtualCamera;
        [SerializeField] private CinemachineVirtualCamera _aimVirtualCamera;

        [SerializeField] private float _normalSensitivity;
        [SerializeField] private float _aimingSensitivity;

        [SerializeField] private LayerMask _aimingColliderMask = new LayerMask();

        [SerializeField] private Transform _debugTransform;
        [SerializeField] private Transform _pfBulletProjectile;
        [SerializeField] private Transform _spawnBulletPosition;

        private InputSystemManager _input;
        private GeneralPlayerController _general;

        [SerializeField] private Animator _animator;

        private Vector3 mouseWorldPosition = Vector3.zero;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _input = GetComponent<InputSystemManager>();
            _general = GetComponent<GeneralPlayerController>();
            _scopingCanvas = GetComponentInChildren<Canvas>();
        }

        private void Update()
        {
            DebugRaycasting();
            Aiming();
            Attacking();
            
        }

        private void DebugRaycasting()
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, _aimingColliderMask))
            {
                _debugTransform.position = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
            }
        }

        private void Aiming()
        {
            if (_input.aiming)
            {
                _animator.SetLayerWeight(0, Mathf.Lerp(_animator.GetLayerWeight(1), 0.5f, Time.deltaTime * 10f));
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
                _aimVirtualCamera.gameObject.SetActive(true);
                _scopingCanvas.gameObject.SetActive(true);
                _debugTransform.gameObject.SetActive(true);
                _general.SetSensitivity(_aimingSensitivity);
                _general.SetRotateOnMove(false);

                Vector3 worldAimingTarget = mouseWorldPosition;
                worldAimingTarget.y = transform.position.y;
                Vector3 aimingDirection = (worldAimingTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimingDirection, Time.deltaTime * 20f);
            }
            else
            {
                _animator.SetLayerWeight(0, Mathf.Lerp(_animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
                _animator.SetLayerWeight(1, Mathf.Lerp(_animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
                _aimVirtualCamera.gameObject.SetActive(false);
                _scopingCanvas.gameObject.SetActive(false);
                _debugTransform.gameObject.SetActive(false);
                _general.SetSensitivity(_normalSensitivity);
                _general.SetRotateOnMove(true);
            }
        }

        private void Attacking()
        {
            if (_input.attack)
            {
                Vector3 aimDir = (mouseWorldPosition - _spawnBulletPosition.position).normalized;
                Instantiate(_pfBulletProjectile, _spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
                _input.attack = false;
            }
        }

    }
}
