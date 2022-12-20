const fs = require('fs');
let items = fs.readFileSync('day20.txt', 'utf8').split('\r\n').map(Number);

const list = items.map((val, i) => ({ val, i }));


for (let x = 0; x < list.length; x++) {
    const currentInd = list.findIndex(({ i }) => i === x % list.length);
    const [el] = list.splice(currentInd, 1);
    list.splice((currentInd + el.val) % list.length, 0, el);
}


const zeroIndex = list.findIndex(el => el.val === 0);
const keys = [1000, 2000, 3000].map(x => list[(zeroIndex + x) % list.length].val);
console.log(keys.reduce((prev, curr) => prev + curr));