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

				else
				{
					if(significant == 0)
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

					if(significant > 0)
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
			return Minified(vector3.x, 4) + "~" + Minified(vector3.y, 4) + "~" + Minified(vector3.z, 4);
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

		public static string ToQuaternionStr(Quaternion rotation)
		{
			return Minified(rotation.eulerAngles.x, 0) + "~" + Minified(rotation.eulerAngles.y, 0) + "~" + Minified(rotation.eulerAngles.z, 0);
		}

		public static Quaternion ToQuaternion(string vectorStr)
		{
			string[] vectorValues = vectorStr.Split(new string[] { "~" }, StringSplitOptions.RemoveEmptyEntries);

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
	}
}