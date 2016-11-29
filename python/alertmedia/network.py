import requests
import json


class Network():
    auth = ""

    def __init__(self, auth):
        self.auth = auth

    def get_url(self, end_point):
        return self.auth.server + end_point

    def build_header(self, start=None, end=None):
        header = {"Authorization": self.auth.authorization, "Content-Type": "application/json"}
        if start is not None and end is not None:
            header["Range"] = "items={}-{}".format(start, end)

        return header

    def get(self, end_point, payload, start=None, end=None):
        r = requests.get(self.get_url(end_point), headers=self.build_header(start, end), params=payload)
        return Result(r)

    def post(self, end_point, payload):
        payload = json.dumps(payload)
        r = requests.post(self.get_url(end_point), headers=self.build_header(), data=payload)
        return Result(r)

    def put(self, end_point, payload):
        payload = json.dumps(payload)
        r = requests.put(self.get_url(end_point), headers=self.build_header(), data=payload)
        return Result(r)

    def delete(self, end_point):
        r = requests.delete(self.get_url(end_point), headers=self.build_header())
        return Result(r)

    def upload(self, end_point, files):
        headers = {"Authorization": self.auth.authorization}
        r = requests.post(self.get_url(end_point), headers=headers, files=files)
        return Result(r)


class Result:

    def __init__(self, response):

        if response.content:
            self.data = response.json()

        # range headers
        item_range = {}
        if 'item-range' in response.headers:
            if response.headers['item-range'] == "items */0":
                item_range['start'] = 0
                item_range['end'] = 0
                item_range['total'] = 0
            else:
                # in the format `items 0-9/100`
                items_part = response.headers['item-range'].split(' ')[1]
                range_part = items_part.split('/')[0]
                item_range['start'] = int(range_part.split('-')[0])
                item_range['end'] = int(range_part.split('-')[1])
                item_range['total'] = int(items_part.split('/')[1])

        self.item_range = item_range
        self.status_code = response.status_code
        self.reason = response.reason
        self.response = response

    def __str__(self):
        return self.__dict__.__str__()

    def __unicode__(self):
        return self.__dict__.__unicode__()