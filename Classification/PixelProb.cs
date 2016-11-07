using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Classification
{
	class PixelProb {
        public double[] prob { get; set; }
        public double[] pixelon { get; set; }
		public PixelProb(){
			
		}
		
		public PixelProb(int size){
			this.prob = new double[size];
            this.pixelon = new double[size];
		}
	}
}