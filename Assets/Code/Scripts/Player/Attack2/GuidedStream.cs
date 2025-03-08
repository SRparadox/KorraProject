using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code modified from https://youtu.be/3CcWus6d_B8?feature=shared

public class GuidedStream: MonoBehaviour
{
    [SerializeField] float pointCount;
    [SerializeField] float radius;
    [SerializeField] Vector3 scale;

    [SerializeField] Spline spline;
    [SerializeField] ExampleContortAlong contortAlong;

    [SerializeField] float speedDelta;
    [SerializeField] float animSpeed;

    [SerializeField] ParticleSystem puddleParticle;
    [SerializeField] ParticleSystem splashParticle;

    [SerializeField] float splashActivationOffset;
    [SerializeField] float puddleScaleSpeed;
    [SerializeField] float hitDetectionRadius;

    private Vector3 target;
    private float damageAmount;
    private CharacterClass player;

    public void SendTo(Vector3 target)
    {
        this.target = target;

        StopAllCoroutines();
        StartCoroutine(Coroutine_SendTo());
    }

    IEnumerator Coroutine_SendTo()
    {
        spline.gameObject.SetActive(false);
        splashParticle.gameObject.SetActive(false);
        puddleParticle.gameObject.SetActive(false);

        ConfigureSpline();
        contortAlong.Init();

        float meshLength = contortAlong.MeshBender.Source.Length;
        meshLength = meshLength == 0 ? 1 : meshLength;
        float totalLength = meshLength + spline.Length;

        Vector3 startScale = scale;
        startScale.x = 0;
        Vector3 targetScale = scale;

        float speedCurveLerp = 0;
        float length = 0;

        puddleParticle.gameObject.SetActive(true);
        puddleParticle.transform.localPosition = spline.nodes[0].Position;

        Vector3 startPuddleScale = Vector3.zero;
        Vector3 endPuddleScale = puddleParticle.transform.localScale;

        float lerp = 0;
        while (lerp < 1)
        {
            puddleParticle.transform.localScale = Vector3.Lerp(startPuddleScale, endPuddleScale, lerp);
            lerp += Time.deltaTime * puddleScaleSpeed;
            yield return null;
        }

        spline.gameObject.SetActive(true);
        puddleParticle.Play();

        while (length < totalLength)
        {
            if (length < meshLength)
            {
                contortAlong.ScaleMesh(Vector3.Lerp(startScale, targetScale, length / meshLength));
            } else
            {
                if (puddleParticle.isPlaying)
                {
                    puddleParticle.Stop();
                }

                contortAlong.Contort((length - meshLength) / spline.Length);
                if (length + meshLength > (totalLength + splashActivationOffset))
                {
                    if (!splashParticle.isPlaying)
                    {
                        CheckForEnemyHit();

                        splashParticle.gameObject.SetActive(true);
                        splashParticle.transform.position = target;
                        splashParticle.Play();
                    }
                }
            }

            length += Time.deltaTime * animSpeed * speedCurveLerp;
            speedCurveLerp += speedDelta * Time.deltaTime;
            yield return null;
        }

        spline.gameObject.SetActive(false);
        splashParticle.Stop();
        Destroy(gameObject, 2f);
    }

    private void ConfigureSpline()
    {
        List<SplineNode> nodes = new List<SplineNode>(spline.nodes);
        for (int i = 2; i < nodes.Count; i++)
        {
            spline.RemoveNode(nodes[i]);
        }

        Vector3 targetDirection = (target - transform.position);
        transform.forward = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;

        int sign = Random.Range(0, 2) == 0 ? 1 : -1;
        float angle = 90 * sign;
        float streamSpawnHeight = transform.position.y;

        for (int i = 0; i < pointCount; i++)
        {
            if (spline.nodes.Count <= i)
            {
                spline.AddNode(new SplineNode(Vector3.zero, Vector3.forward));
            }

            Vector3 normal = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 pos = transform.position + normal * radius;
            pos.y = streamSpawnHeight;

            Vector3 direction = pos + Quaternion.Euler(Random.Range(-30, 30), Random.Range(60, 120) * sign, Random.Range(-30, 30)) * normal * radius / 2f;

            if (i == 0)
            {
                direction = pos + Vector3.up * radius;
            }

            spline.nodes[i].Position = transform.InverseTransformPoint(pos);
            spline.nodes[i].Direction = transform.InverseTransformPoint(direction);

            angle += 90 * sign;
        }

        Vector3 targetNodePosition = transform.InverseTransformPoint(target);

        Quaternion randomRotation = Quaternion.Euler(Random.Range(0, 90), Random.Range(-40, 40), 0);
        Vector3 targetNodeDirection = target + randomRotation * (transform.forward * (target - transform.position).magnitude * Random.Range(0.2f, 1f));

        targetNodeDirection = transform.InverseTransformPoint(targetNodeDirection);
        SplineNode node = new SplineNode(targetNodePosition, targetNodeDirection);
        spline.AddNode(node);
    }

    public void setDamage(float damage){
        damageAmount = damage;
    }

    public void SetPlayer(CharacterClass character)
    {
        player = character;
    }


    private void CheckForEnemyHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(target, hitDetectionRadius);
        HashSet<GameObject> damagedObjects = new HashSet<GameObject>();

        foreach (Collider hit in hitColliders)
        {
            GameObject hitObject = hit.gameObject;

            if (hitObject.GetComponent<CharacterClass>() != null && !damagedObjects.Contains(hitObject) && hitObject.tag != gameObject.tag)
            {
                hitObject.GetComponent<CharacterClass>().TakeDamage(damageAmount * player.getDamageMultiplier());
                damagedObjects.Add(hitObject);

                if (player != null)
                {
                    player.OnSuccessfulHit();
                }
                return;
            }
        }
    }

}
