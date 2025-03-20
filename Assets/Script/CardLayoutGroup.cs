using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

/// <summary>
/// �J�[�h�𕡐��z�u����ۂ�LayoutGroup
/// </summary>
/// <remarks>
/// �q�G�����L�[��ō��W�𓯒肷��̂��ړI�ł���APlay���͏d���̂Ŏg��Ȃ�
/// </remarks>
public class CardLayoutGroup : LayoutGroup
{
    /// <summary>
    /// �z�u���@�̃p�^�[��
    /// </summary>
    public enum AlignmentType
    {
        Offset,  // �n�_���珇��OffsetAngle���Ƃɓ��Ԋu�Ŕz�u
        StartEnd // �n�_�p�x/�I�_�p�x���w�肵�āA���̒��ň����ꂽ�ȉ~��̍��W�ɔz�u
    }

    /// <summary>
    /// �I�u�W�F�N�g��Rotation��ύX������@
    /// </summary>
    public enum RotationType
    {
        None,           // �I�u�W�F�N�g��Rotation�͕ύX���Ȃ�
        NormalVector,   // �@���x�N�g��������Rotation��ύX
        LookAt,         // �v�f��Y���W����������̍��W�������悤�ɕύX
        Offset,         // �v�f���ɉ����ē���̊p�x����Rotation��ύX
        StartEnd        // �n�_Rotation/�I�_Rotation���w�肵�Ĉ����ꂽ�p�x�ŉ�]
    }

    /// <summary>
    /// �v�f�̔z�u����
    /// </summary>
    public enum Direction
    {
        Clockwise,       // ���v���
        CounterClockwise // �����v���
    }

    /// <summary>
    /// �ȉ~���������Č����邽�߂̃p�����[�^
    /// </summary>
    [Serializable]
    private class CircleParameter
    {
        /// <summary>
        /// �ȉ~������ x^2/a^2 + y^2/b^2 = 1��a�̒l
        /// </summary>
        [SerializeField]
        private float a;
        public float A { get => this.a; }

        /// <summary>
        /// �ȉ~������ x^2/a^2 + y^2/b^2 = 1��b�̒l
        /// </summary>
        [SerializeField]
        private float b;
        public float B { get => this.b; }

        /// <summary>
        /// �}��ϐ����W(acos��, bsin��)�ɂ����钆�S����̋���
        /// </summary>
        [SerializeField]
        private float distance;
        public float Distance { get => this.distance; }
    }

    [SerializeField]
    private AlignmentType alignmentType;
    public AlignmentType Alignment { get => this.alignmentType; }

    [SerializeField]
    private Direction alignmentDirection;

    [SerializeField]
    private RotationType rotationType;
    public RotationType Rotation { get => this.rotationType; }

    [SerializeField]
    private CircleParameter circleParameter;

    /// <summary>
    /// �v�f��z�u����ۂ̎n�_�̃I�C���[�p
    /// </summary>
    [Range(0f, 360f)]
    public float startAngle;

    /// <summary>
    /// �v�f��z�u����ۂ̃I�C���[�p�̍���
    /// </summary>
    [SerializeField]
    [Range(0f, 360f)]
    [HideInInspector]
    private float offsetAngle;

    /// <summary>
    /// �v�f��z�u����ۂ̏I�_�̃I�C���[�p
    /// </summary>
    [Range(0f, 360f)]
    public float endAngle;

    /// <summary>
    /// �z�u���ꂽ�v�f��Rotation�������������
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private Vector2 lookAt;

    /// <summary>
    /// �v�f����]������ꍇ��Rotation�̎n�_
    /// </summary>
    [SerializeField]
    [HideInInspector]
    [Range(-180f, 180f)]
    private float startRotation;

    /// <summary>
    /// �v�f����]������ꍇ��Rotation�̍���
    /// </summary>
    [SerializeField]
    [HideInInspector]
    private float offsetRotation;

    /// <summary>
    /// �v�f����]������ꍇ��Rotation�̏I�_
    /// </summary>
    [SerializeField]
    [HideInInspector]
    [Range(-180f, 180f)]
    private float endRotation;

    public void Calclate()
    {
        m_Tracker.Clear();

        List<GameObject> childrenList = GetChildrenWithoutGrandchildren();
        List<RectTransform> childs = new List<RectTransform>();

        foreach(var child in childrenList)
        {
            childs.Add(child.GetComponent<RectTransform>());
        }
        childrenList.Clear();

        var childCount = childs.Count;
        if (childCount == 0) return;

        for (int i = 0; i < childCount; i++)
        {
            var child = childs[i];

            m_Tracker.Add(this, child, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Pivot);
            // �v�f���ɑ΂����
            var divide = (childCount > 1) ? (float)i / (float)(childCount - 1) : 0.0f;
            // �z�u�J�n�ʒu�����offset���v�Z
            var deltaAngle = this.alignmentType switch
            {
                AlignmentType.Offset => i * this.offsetAngle,
                AlignmentType.StartEnd => Math.Abs(this.endAngle - this.startAngle) * divide,
            };
            // �v�fn�ڂ̃I�C���[�p
            var nAngle = this.startAngle + (this.alignmentDirection == Direction.Clockwise ? -deltaAngle : deltaAngle);
            // �ȉ~��̍��W (acos��, bsin��)
            var nPosition = new Vector2(this.circleParameter.A * Mathf.Cos(nAngle * Mathf.Deg2Rad), this.circleParameter.B * Mathf.Sin(nAngle * Mathf.Deg2Rad));
            child.anchoredPosition = nPosition * this.circleParameter.Distance;

            switch (this.rotationType)
            {
                case RotationType.None:
                    // �I�u�W�F�N�g�̉�]���s��Ȃ�
                    child.rotation = Quaternion.identity;
                    break;
                case RotationType.NormalVector:
                    // �ȉ~��̓_�̖@���x�N�g����(bcos��, asin��)
                    var normalVector = new Vector2(this.circleParameter.B * Mathf.Cos(nAngle * Mathf.Deg2Rad), this.circleParameter.A * Mathf.Sin(nAngle * Mathf.Deg2Rad));
                    child.rotation = Quaternion.AngleAxis(Mathf.Atan2(normalVector.x, normalVector.y) * Mathf.Rad2Deg, Vector3.back);
                    break;
                case RotationType.LookAt:
                    // �J�[�h��Y���W��������ɉ�]������
                    var lookVector = child.anchoredPosition - this.lookAt;
                    child.rotation = Quaternion.AngleAxis(Mathf.Atan2(lookVector.x, lookVector.y) * Mathf.Rad2Deg, Vector3.back);
                    break;
                case RotationType.Offset:
                    // Rotation�͍���]����
                    var deltaRotation = i * this.offsetRotation;
                    child.rotation = Quaternion.Euler(0, 0, this.startRotation - deltaRotation);
                    break;
                case RotationType.StartEnd:
                    // ����Quaternion���Z�o
                    child.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, this.startRotation), Quaternion.Euler(0, 0, this.endRotation), divide);
                    break;
            }
            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
        }
        childs.Clear();
    }

    public List<GameObject> GetChildrenWithoutGrandchildren()
    {
        var result = new List<GameObject>();
        foreach (Transform n in this.gameObject.transform)
        {
            result.Add(n.gameObject);
        }
        return result;
    }

    protected override void OnEnable()
    { 
        base.OnEnable();
        Calclate(); 
    }
    public override void SetLayoutHorizontal()
    {
    }
    public override void SetLayoutVertical()
    {
    }
    public override void CalculateLayoutInputVertical()
    {
        //Calclate();
    }
    public override void CalculateLayoutInputHorizontal()
    {
        //Calclate();
    }
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        Calclate();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(CardLayoutGroup))]
public class CardLayoutGroupEditor : Editor
{
    private CardLayoutGroup _target;
    private SerializedProperty offsetAngle;
    private SerializedProperty endAngle;
    private SerializedProperty lookAt;
    private SerializedProperty startRotation;
    private SerializedProperty offsetRotation;
    private SerializedProperty endRotation;

    private void OnEnable()
    {
        _target = target as CardLayoutGroup;

        this.offsetAngle = serializedObject.FindProperty("offsetAngle");
        this.endAngle = serializedObject.FindProperty("endAngle");
        this.lookAt = serializedObject.FindProperty("lookAt");
        this.startRotation = serializedObject.FindProperty("startRotation");
        this.offsetRotation = serializedObject.FindProperty("offsetRotation");
        this.endRotation = serializedObject.FindProperty("endRotation");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.UpdateIfRequiredOrScript();
        var iterator = this.serializedObject.GetIterator();
        for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
        {
            if (iterator.propertyPath == "m_Padding" || iterator.propertyPath == "m_ChildAlignment") continue;
            using (new EditorGUI.DisabledScope(iterator.propertyPath == "m_Script"))
            {
                EditorGUILayout.PropertyField(iterator, true);
            }
        }

        switch (_target.Alignment)
        {
            case CardLayoutGroup.AlignmentType.Offset:
                EditorGUILayout.PropertyField(this.offsetAngle);
                break;
            case CardLayoutGroup.AlignmentType.StartEnd:
                EditorGUILayout.PropertyField(this.endAngle);
                break;
        }

        switch (_target.Rotation)
        {
            case CardLayoutGroup.RotationType.LookAt:
                EditorGUILayout.PropertyField(this.lookAt);
                break;
            case CardLayoutGroup.RotationType.Offset:
                EditorGUILayout.PropertyField(this.startRotation);
                EditorGUILayout.PropertyField(this.offsetRotation);
                break;
            case CardLayoutGroup.RotationType.StartEnd:
                EditorGUILayout.PropertyField(this.startRotation);
                EditorGUILayout.PropertyField(this.endRotation);
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
