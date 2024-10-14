import express from 'express';
const axios = require('axios');

const app = express();
const port = 3000;

app.use(express.json())

const bidders = [
  {
    name: 'cereal_1',
    url: 'http://localhost:3001',
    willingness_to_pay: 90,
    target_attention: 0.9,
  },
  {
    name: 'cereal_2',
    url: 'http://localhost:3002',
    willingness_to_pay: 87,
    target_attention: 0.8,
  },
  {
    name: 'cereal_3',
    url: 'http://localhost:3003',
    willingness_to_pay: 70,
    target_attention: 0.7,
  },
];

app.get('/', (req, res) => {
  res.send('Hello, world!');
});

app.listen(port, () => {
  console.log(`Server is running on port ${port}`);
});

/*
 * This function takes a json heatmap in the form {"object_name": "attention percentage (%)", "object_name": "attention percentage (%)", ...}
 * and starts an auction for the bidders to bid on the difference in attention percentage they want to achieve (i.e., 
 * if I am the object "cereal_2" and have currently an attention percentage of 30%, but need 50%, I would bid on a 20% point increase, thus
 * my offer is divided by 20).
 */
app.post('/startAuction', (req, res) => {

  console.log("Body: ", req.body);

  // Get the heatmap from the request
  const heatmap = req.body.heatmap;

  // Start the auction
  // Compare the heatmap with the target attention of each bidder

  let offers : any[] = [];

  // For each bidder
  for (let bidder of bidders) {
    // Get the attention percentage of the object the bidder is interested in
    let object_attention = heatmap[bidder.name];

    // If the object was not found in the heatmap, continue with the next bidder
    if (!object_attention) {
      object_attention = 0;
      //continue;
    }

    // If the attention percentage is lower than the target attention
    if (object_attention < bidder.target_attention) {
      // Calculate the difference in attention percentage
      const difference = (bidder.target_attention - object_attention) * 100;

      // Divide the willingness_to_pay by the difference in attention percentage
      // Let the willingness to pay be +/- 20% randomly
      let wtp = bidder.willingness_to_pay + (Math.random() * 40 - 20);

      const offer = {"bidder": bidder, "offer": wtp / difference};
      offer.bidder.willingness_to_pay = wtp;

      // Add the offer to the offers array
      offers.push(offer);
    }
  }

  // Response content type JSON
  res.setHeader('Content-Type', 'application/json');

  // If there are no offers, return an empty object
  if (offers.length == 0) {
    console.log("Auction completed with no offers!");
    res.send({});
    return;
  }

  // Find the highest offer
  let highest_offer = offers[0];
  for (let offer of offers) {
    if (offer.offer > highest_offer.offer) {
      highest_offer = offer;
    }
  }

  console.log("Auction completed with highest offer: ", highest_offer);

  // Send a get request http://130.82.25.64:5050/?aoi_{highest_offer_bidder}=1&duration=3000
  // Send a get request
  axios.get(`http://130.82.25.64:5050/?aoi_${highest_offer.bidder.name}=1&duration=3000`)
  .then((response: any) => {
    // handle success
    console.log(response.data);
  })
  .catch((error: any) => {
    // handle error
    console.log(error);
  });

  // Return the highest offer
  res.send(highest_offer);

});