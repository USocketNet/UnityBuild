using System;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Serializables
{
	[System.Serializable]
	public class VectorJson
	{
		public float x = 0f;
		public float y = 0f;
		public float z = 0f;

		private static string Minified(float floatings, int significant)
		{
			string final = "";
			bool foundDot = false;
			int countingSigna = 0;
			string axis = floatings.ToString ();
			for (int i = 0; i < axis.Length; i++)
			{
				if(foundDot)
				{
					if(significant <= 0)
					{
						break;
					}

					else
					{
						if(countingSigna < significant) //significant
						{
							final = final + axis [i];
							countingSigna += 1;
						}

						else
						{
							break;
						}
					}
				}

				else
				{
					if(significant <= 0)
					{
						if(axis[i] == '.')
						{
							if(axis.ToLower().IndexOf('e') != -1)
							{
								bool foundEs = false;
								for (int it = 0; it < axis.Length; it++)
								{
									if(axis[it] == 'E')
									{
										foundEs = true;
									}

									if(foundEs)
									{
										final = final + axis [it];
									}
								}

								break;
							}

							else
							{
								break;
							}
						}

						else
						{
							final = final + axis [i];
						}
					}

					else
					{
						if(axis.ToLower().IndexOf('e') != -1)
						{
							if(axis[i] == '.')
							{
								bool foundEs = false;
								for (int it = i; it < axis.Length; it++)
								{
									if(axis[it] == 'E')
									{
										foundEs = true;
									}

									if(foundEs)
									{
										final = final + axis [it];
									}
								}

								break;
							}

							else
							{
								final = final + axis [i];
							}

						}

						else
						{
							final = final + axis [i];

							if(axis[i] == '.')
							{
								foundDot = true;
							}
						}
					}
				}
			}

			return final;
		}

		public static string ToVectorStr(Vector3 vector3)
		{
			return Minified(vector3.x, 7) + "~" + Minified(vector3.y, 7) + "~" + Minified(vector3.z, 7);
		}

		public static string ToVectorStr(Vector3 vector3, SocketAxis axises, Floatings significants)
		{
			string vString = string.Empty;

			if(axises.xAxis)
			{
				vString = Minified(vector3.x, significants.xPoints);
			}

			if(axises.yAxis)
			{
				if(vString != string.Empty)
				{
					vString = vString + "~";
				}

				vString = vString + Minified(vector3.y, significants.yPoints) ;
			}

			if(axises.zAxis)
			{
				if(vString != string.Empty)
				{
					vString = vString + "~";
				}

				vString = vString + Minified(vector3.z, significants.zPoints);
			}

			return vString;
		}

		public static Vector3 ToVector3(string vectorStr)
		{
			string[] vectorValues = vectorStr.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);


			if (vectorValues.Length == 3)
			{
				return new Vector3
					(
						Convert.ToSingle(vectorValues[0]),
						Convert.ToSingle(vectorValues[1]),
						Convert.ToSingle(vectorValues[2])
					);
			}

			else
			{
				return Vector3.zero;
			}
		}

		public static Vector3 ToVector3(string vectorStr, Vector3 position, SocketAxis axises)
		{
			string[] vectorValues = vectorStr.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

			Vector3 vectors = position;
			int curAxis = 0;

			if(axises.xAxis)
			{
				vectors.x = Convert.ToSingle(vectorValues[curAxis]);
				curAxis += 1;
			}

			if(axises.yAxis)
			{
				vectors.y = Convert.ToSingle(vectorValues[curAxis]);
				curAxis += 1;
			}

			if(axises.zAxis)
			{
				vectors.z = Convert.ToSingle(vectorValues[curAxis]);
			}

			return vectors;
		}

		public static string ToQuaternionStr(Quaternion rotation)
		{
			return Minified(rotation.eulerAngles.x, 7) + "~" + Minified(rotation.eulerAngles.y, 7) + "~" + Minified(rotation.eulerAngles.z, 7);
		}

		public static string ToQuaternionStr(Quaternion rotation, SocketAxis axises, Floatings significants)
		{
			string qString = string.Empty;

			if(axises.xAxis)
			{
				qString = qString + Minified(rotation.eulerAngles.x, significants.xPoints);
			}

			if(axises.yAxis)
			{
				if(qString != string.Empty)
				{
					qString = qString + "~";
				}

				qString = qString + Minified(rotation.eulerAngles.y, significants.yPoints) ;
			}

			if(axises.zAxis)
			{
				if(qString != string.Empty)
				{
					qString = qString + "~";
				}

				qString = qString + Minified(rotation.eulerAngles.z, significants.zPoints);
			}

			return qString;
		}

		public static Quaternion ToQuaternion(string eulerString)
		{
			string[] vectorValues = eulerString.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

			if (vectorValues.Length == 3)
			{
				Vector3 eulerValue = new Vector3
					(
						Convert.ToSingle(vectorValues[0]),
						Convert.ToSingle(vectorValues[1]),
						Convert.ToSingle(vectorValues[2])
					);

				return Quaternion.Euler(eulerValue);
			}

			else
			{
				return Quaternion.identity;
			}
		}

		public static Quaternion ToQuaternion(string eulerString, Quaternion rotation, SocketAxis axises)
		{
			string[] eulerValues = eulerString.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

			Vector3 eulerAngle = rotation.eulerAngles;
			int curAxis = 0;

			if(axises.xAxis)
			{
				eulerAngle.x = Convert.ToSingle(eulerValues[curAxis]);
				curAxis += 1;
			}

			if(axises.yAxis)
			{
				eulerAngle.y = Convert.ToSingle(eulerValues[curAxis]);
				curAxis += 1;
			}

			if(axises.zAxis)
			{
				eulerAngle.z = Convert.ToSingle(eulerValues[curAxis]);
			}

			return Quaternion.Euler(eulerAngle);
		}
	}
}