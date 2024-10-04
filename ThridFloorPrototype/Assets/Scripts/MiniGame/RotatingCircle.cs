using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCircle : MonoBehaviour
{
    public float baseRotationSpeed = 100f;
    private float rotationSpeed;
    private float reverseTimer;
    private float reverseInterval;

    void Start()
    {
        reverseInterval = UnityEngine.Random.Range(1f, 5f);

        int region = DataCenter.Instance.Region;
        int stage = DataCenter.Instance.Stage;

        string csvData = DataCenter.Instance.GetCsvFile().text;

        string[] lines = csvData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            string[] values = line.Split(',');

            if (values.Length < 4)
            {
                Debug.LogWarning("Invalid CSV line, skipping: " + line);
                continue;
            }

            if (!int.TryParse(values[0], out int csvRegion) ||
                !int.TryParse(values[1], out int csvStage) ||
                !int.TryParse(values[2], out int level) ||
                !int.TryParse(values[3], out int typeGame))
            {
                Debug.LogWarning("Failed to parse integers from CSV line, skipping: " + line);
                continue;
            }

            if (csvRegion == region && csvStage == stage && typeGame == 2)
            {
                rotationSpeed = baseRotationSpeed + level;
                break;
            }
        }
    }


    void Update()
    {
        if (DataCenter.Instance.GetPlayerData().region >= 2)
        {
            reverseTimer += Time.deltaTime;

            if (reverseTimer >= reverseInterval)
            {
                rotationSpeed = -rotationSpeed; 
                reverseTimer = 0f;
                reverseInterval = UnityEngine.Random.Range(1f, 5f);
            }
        }

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
