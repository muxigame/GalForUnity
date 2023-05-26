

using System;
using GalForUnity.Core.Editor;
using UnityEngine;

namespace GalForUnity.Framework {
	/// <summary>
	/// GameTime是对DateTime的一个封装，允许你操作时间对象，在未来会切换为自己的API，以给足更多的自由度
	/// </summary>
	[Serializable]
	public class GameTime{
		
		public override int GetHashCode(){
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
		
		public static GameTime Zero = new GameTime(2020, 12, 1);

		public GameTime(){
			var utcNow = DateTime.UtcNow;
			var localTime = utcNow.ToLocalTime();
			this.year = localTime.Year;
			this.month = localTime.Month;
			this.day = localTime.Day;
		}

		public GameTime(int year, int month, int day){
			this.year = year;
			this.month = month;
			this.day = day;
		}
		public GameTime(int year, int month) : this(year, month, 0) { }

		[Rename(nameof(year))]
		public int year = 2020;
		[Rename(nameof(month))]
		public int month = 12;
		[Rename(nameof(day))]
		public int day = 1;

		public GameTime NextDay(){
			return this + new GameTime(0, 0, 1);
		}
		

		public static GameTime operator+(GameTime gameTime1, GameTime gameTime2) {
			var time = new DateTime(gameTime1.year,gameTime1.month,gameTime1.day);
			time = time.AddDays(gameTime2.day);
			time = time.AddMonths(gameTime2.month);
			time = time.AddYears(gameTime2.year);
			return new GameTime(time.Year, time.Month , time.Day);
		}
		public static GameTime operator+(GameTime gameTime1, int daycount) {
			var time = new DateTime(gameTime1.year,gameTime1.month,gameTime1.day);
			time = time.AddDays(daycount);
			return new GameTime(time.Year, time.Month , time.Day);
		}
		public static GameTime operator-(GameTime gameTime1, GameTime gameTime2) {
			var time = new DateTime(gameTime1.year,gameTime1.month,gameTime1.day);
			time=time.AddMonths(-gameTime2.month);
			time=time.AddDays(-gameTime2.day);
			return new GameTime(gameTime1.year-gameTime2.year, time.Month, time.Day);
		}
		public static bool operator<(GameTime gameTime1, GameTime gameTime2) {
			if(gameTime1.year<gameTime2.year){
				return true;
			}else if(gameTime1.year == gameTime2.year){
				if (gameTime1.month < gameTime2.month) {
					return true;
				} else if(gameTime1.month == gameTime2.month) {
					if (gameTime1.day < gameTime2.day) {
						return true;
					}
				}
			}
			return false;
		}
		public static bool operator>(GameTime gameTime1, GameTime gameTime2) {
			if(gameTime1.year>gameTime2.year){
				return true;
			}else if(gameTime1.year == gameTime2.year){
				if (gameTime1.month > gameTime2.month) {
					return true;
				} else if(gameTime1.month == gameTime2.month) {
					if (gameTime1.day > gameTime2.day) {
						return true;
					}
				}
			}
			return false;
		}
		public static bool operator <=(GameTime gameTime1, GameTime gameTime2) {
			if (gameTime1.year < gameTime2.year) {
				return true;
			} else if (gameTime1.year == gameTime2.year) {
				if (gameTime1.month < gameTime2.month) {
					return true;
				} else if (gameTime1.month == gameTime2.month) {
					if (gameTime1.day <= gameTime2.day) {
						return true;
					}
				}
			}
			return false;
		}
		public static bool operator>=(GameTime gameTime1, GameTime gameTime2) {
			if(gameTime1.year>gameTime2.year){
				return true;
			}else if(gameTime1.year == gameTime2.year){
				if (gameTime1.month > gameTime2.month) {
					return true;
				} else if(gameTime1.month == gameTime2.month) {
					if (gameTime1.day >= gameTime2.day) {
						return true;
					}
				}
			}
			return false;
		}
		public override bool Equals(object obj) {
			if (!(obj is GameTime)) {
				return false;
			}
			GameTime gameTime = obj as GameTime;
			return year == gameTime.year && month == gameTime.month && day == gameTime.day;
		}
				
		protected bool Equals(GameTime other)
		{
			return year == other.year && month == other.month && day == other.day;
		}

		public static bool operator == (GameTime gameTime1, GameTime gameTime2) {
			return !(gameTime1 is null) && !(gameTime2 is null) && gameTime1.year == gameTime2.year && gameTime1.month == gameTime2.month && gameTime1.day == gameTime2.day;
		}
		public static bool operator != (GameTime gameTime1, GameTime gameTime2) {
			return !(gameTime1==gameTime2);
		}

		public override string ToString() {
			return year+"年"+month+"月"+day+"日";
		}

		public static implicit operator Vector3Int(GameTime gameTime){
			return new Vector3Int(gameTime.year,gameTime.month,gameTime.day);
		}
		
		public static implicit operator GameTime(Vector3Int vector3Time){
			return new GameTime((int)vector3Time.x,(int)vector3Time.y,(int)vector3Time.z);
		}
	}
}
