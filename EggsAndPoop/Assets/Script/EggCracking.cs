using UnityEngine;

public class EggCracking : MonoBehaviour
{

    public Transform explosionOrigin;
    public float explosionForce;
    public float explosionRadius;

    public Rigidbody[] rigidBodies;

    public int currentHp;

    public int crackSections;
    public int crackAmount;

    public GameObject tapVfx;
    public GameObject explosionVfx;

    public ParticleSystem[] ambientVfx;

    private Camera eggCamera;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DoRaycast();
        }
    }

    private void DoRaycast()
    {
        RaycastHit hit;
        Ray ray = eggCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (RecursiveHitFind(hit))
            {
                EggTapped(hit.point);
            }
        }
    }

    private bool RecursiveHitFind(RaycastHit hit, Transform activeTransform = null)
    {
        if (activeTransform == null)
        {
            activeTransform = hit.transform;
        }

        if (activeTransform.CompareTag("Egg"))
        {
            return true;
        }

        if (activeTransform.parent != null)
        {
            return RecursiveHitFind(hit, activeTransform.parent);
        }

        return false;
    }

    public void EggTapped(Vector3 tapLocation)
    {

        if (currentHp <= 0)
        {
            return;
        }

        currentHp--;

        if (currentHp <= 0)
        {
            ExplodeEgg();
            return;
        }

        Instantiate(tapVfx, tapLocation, Quaternion.identity);
        CrackEgg();
        CrackEggTest();
    }

    public void CrackEgg()
    {
        int crackIndex = Random.Range(0, rigidBodies.Length);

        for (int i = 0; i < crackSections; i++)
        {
            var t = transform.GetChild(crackIndex);

            if (i % 2 == 0)
            {
                t.Rotate(Vector3.up, crackAmount / 2f);
            }

            else
            {
                t.Rotate(Vector3.up, -crackAmount / 2f);
            }

            if (i % 3 == 0)
            {
                t.Rotate(Vector3.right, crackAmount);
            }

            else
            {
                t.Rotate(Vector3.right, -crackAmount);
            }

            crackIndex++;
            crackIndex = crackIndex % rigidBodies.Length;
        }
    }

    public void CrackEggTest()
    {
        for (int i = 0; i < 2; i++)
        {
            var rand = Random.Range(0, rigidBodies.Length);
            var rb = rigidBodies[rand];
            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce / 2f, explosionOrigin.position, explosionRadius);
        }

    }

    public void ExplodeEgg()
    {
        foreach (var rb in rigidBodies)
        {
            rb.isKinematic = false;
            rb.AddExplosionForce(explosionForce, explosionOrigin.position, explosionRadius);
        }

        Instantiate(explosionVfx, explosionOrigin.position, Quaternion.identity);

        foreach (var vfx in ambientVfx)
        {
            vfx.Play();
        }

        GameManager.Instance.m_EggCrackedOpen.Invoke();
    }

    public void SetCamera(Camera eggCamera)
    {
        this.eggCamera = eggCamera;
    }
}
