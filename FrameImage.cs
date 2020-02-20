using System;
using UnityEngine;
using UnityEngine.UI;

namespace Common.TutorialManager
{
	[ExecuteInEditMode]
	public class FrameImage : Graphic
	{
		private readonly Lazy<Mesh> _mesh;
		private readonly Lazy<RectTransform> _rectTransform;

		private readonly Vector3[] _vertices = new Vector3[16];
		private readonly Vector2[] _uvs = new Vector2[16];

		private readonly Vector3[] _normals =
		{
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward
		};

		private readonly int[] _tris =
		{
			0, 4, 1, 4, 5, 1, 1, 5, 2, 5, 6, 2, 2, 6, 3, 6, 7, 3, 4, 8, 5, 8, 9, 5, 5, 9, 6, 9, 10, 6, 6, 10, 7, 10, 11,
			7, 8, 12, 9, 12, 13, 9, 9, 13, 10, 13, 14, 10, 10, 14, 11, 14, 15, 11
		};

#pragma warning disable 649
		[SerializeField] private Sprite _sprite;

		[Header("Frame"), SerializeField] private float _frameXMin = 33f;
		[SerializeField] private float _frameYMin = 33f;
		[SerializeField] private float _frameXMax = 66f;
		[SerializeField] private float _frameYMax = 66f;

		[Header("UI"), SerializeField, Range(0, 1)]
		private float _frameUMin = 0.25f;

		[SerializeField, Range(0, 1)] private float _frameUMax = 0.75f;
		[SerializeField, Range(0, 1)] private float _frameVMin = 0.25f;
		[SerializeField, Range(0, 1)] private float _frameVMax = 0.75f;
#pragma warning restore 649

		public Rect FrameRect
		{
			get => new Rect {xMin = _frameXMin, xMax = _frameXMax, yMin = _frameYMin, yMax = _frameYMax};
			set
			{
				_frameXMin = value.xMin;
				_frameXMax = value.xMax;
				_frameYMin = value.yMin;
				_frameYMax = value.yMax;

				UpdateGeometry();
			}
		}

		public FrameImage()
		{
			_mesh = new Lazy<Mesh>(() =>
			{
				var m = new Mesh();
				m.name = "Shared UI Mesh";
				m.hideFlags = HideFlags.HideAndDontSave;
				return m;
			});

			_rectTransform = new Lazy<RectTransform>(GetComponent<RectTransform>);
		}

		public override Texture mainTexture => _sprite == null ? s_WhiteTexture : _sprite.texture;

		public Sprite Sprite
		{
			get => _sprite;
			set
			{
				_sprite = value;
				UpdateGeometry();
			}
		}

		protected override void UpdateGeometry()
		{
			UpdateMesh();
			canvasRenderer.SetMesh(_mesh.Value);
		}

		private void UpdateMesh()
		{
			var sz = _rectTransform.Value.rect.size;
			var offset = Vector2.Scale(sz, _rectTransform.Value.pivot);

			float uvbXMin, uvbXMax, uvbYMin, uvbYMax;
			if (_sprite != null)
			{
				uvbXMin = float.MaxValue;
				uvbXMax = float.MinValue;
				uvbYMin = float.MaxValue;
				uvbYMax = float.MinValue;

				foreach (var v in _sprite.uv)
				{
					uvbXMin = Mathf.Min(uvbXMin, v.x);
					uvbXMax = Mathf.Max(uvbXMax, v.x);
					uvbYMin = Mathf.Min(uvbYMin, v.y);
					uvbYMax = Mathf.Max(uvbYMax, v.y);
				}
			}
			else
			{
				uvbXMin = 0f;
				uvbXMax = 1f;
				uvbYMin = 0f;
				uvbYMax = 1f;
			}

			var uvbW = uvbXMax - uvbXMin;
			var uvbH = uvbYMax - uvbYMin;

			var px = new[]
			{
				0f,
				Mathf.Min(Mathf.Max(0f, _frameXMin), sz.x),
				Mathf.Min(Mathf.Max(0f, _frameXMin, _frameXMax), sz.x),
				sz.x
			};
			var py = new[]
			{
				0f,
				Mathf.Min(Mathf.Max(0f, _frameYMin), sz.y),
				Mathf.Min(Mathf.Max(0f, _frameYMin, _frameYMax), sz.y),
				sz.y
			};

			var pu = new[]
			{
				0f,
				Mathf.Min(Mathf.Max(0f, _frameUMin), 1f),
				Mathf.Min(Mathf.Max(0f, _frameUMin, _frameUMax), 1f),
				1f
			};

			var pv = new[]
			{
				0f,
				Mathf.Min(Mathf.Max(0f, _frameVMin), 1f),
				Mathf.Min(Mathf.Max(0f, _frameVMin, _frameVMax), 1f),
				1f
			};

			for (var y = 0; y < 4; ++y)
			{
				for (var x = 0; x < 4; ++x)
				{
					var index = x + y * 4;
					_vertices[index] = new Vector3(px[x] - offset.x, py[y] - offset.y, 0);
					_uvs[index] = new Vector2(pu[x] * uvbW + uvbXMin, pv[y] * uvbH + uvbYMin);
				}
			}

			var m = _mesh.Value;
			m.Clear();
			m.vertices = _vertices;
			m.triangles = _tris;
			m.normals = _normals;
			m.uv = _uvs;
		}
	}
}