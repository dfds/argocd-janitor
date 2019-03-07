const express = require("express");
const fs = require("fs");
const util = require("util");

const port = process.env.port || 3000;

const app = express();
app.use(express.json());

const readFile = util.promisify(fs.readFile);
const writeFile = util.promisify(fs.writeFile);
const serialize = (data) => JSON.stringify(data, null, 2);
const deserialize = (text) => JSON.parse(text);

app.post("/api/v1/session", (req, res) => {
    res.status(200).json({token: "token|"+req.body.username+"|"+req.body.password})
});



app.post("/api/v1/projects", (req, res) => {

});


app.listen(port, () => {
    console.log("Fake ArgoCD is listening on port " + port);
});