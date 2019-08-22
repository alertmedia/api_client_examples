import base64
import json
from requests import Session
from collections import namedtuple
import time


class AlertMediaSession(Session):
    def __init__(self, auth):
        super().__init__()
        self.headers["Authorization"] = "Basic " + base64.b64encode(bytes(auth.client_id + ":" + auth.client_key, 'utf-8')).decode()
        self.headers["Content-Type"] = "application/json"

    def request(self, *args, **kwargs):
        while True:
            response = super().request(*args, **kwargs)
            if response.status_code != 429:
                return response
            time.sleep(5)


class Network(object):
    def __init__(self, auth):
        self.session = AlertMediaSession(auth)
        self.server_url = auth.server

    def get_url(self, end_point, query_params=None):
        url = self.server_url + end_point
        if query_params:
            url += "&" if "?" in url else "?"
            url += "&".join("%s=%s" % (k, v,) for k, v in query_params.items())
        return url

    @staticmethod
    def build_header(start=None, end=None):
        if start is not None and end is not None:
            return {"Range": "items={}-{}".format(start, end)}
        return {}

    def list(self, end_point, payload=None, start=None, end=None, query_params=None):
        r = self.session.get(self.get_url(end_point, query_params), headers=self.build_header(start, end), params=payload)
        return PagedResult(r, callback=self.list, callback_kwargs=dict(
            end_point=end_point, payload=payload, start=start, end=end, query_params=query_params))

    def get(self, end_point, payload=None, start=None, end=None, query_params=None):
        r = self.session.get(self.get_url(end_point, query_params=query_params), headers=self.build_header(start, end), params=payload)
        return Result(r)

    def post(self, end_point, payload=None, query_params=None):
        payload = json.dumps(payload)
        r = self.session.post(self.get_url(end_point, query_params), headers=self.build_header(), data=payload)
        return Result(r)

    def put(self, end_point, payload=None, query_params=None):
        payload = json.dumps(payload)
        r = self.session.put(self.get_url(end_point, query_params), headers=self.build_header(), data=payload)
        return Result(r)

    def delete(self, end_point, payload=None, query_params=None):
        r = self.session.delete(self.get_url(end_point, query_params), headers=self.build_header(), data=payload)
        return Result(r)

    def upload(self, end_point, files):
        headers = {"Authorization": self.auth.authorization}
        r = self.session.post(self.get_url(end_point), headers=headers, files=files)
        return Result(r)


class Result(object):
    def __init__(self, response):
        self.response = response
        self.status_code = response.status_code
        self.reason = response.reason

    @property
    def data(self):
        self.response.raise_for_status()
        return self.response.json()

    def get(self, key, default_value=None):
        obj = self.data
        for key in key.split("."):
            if key not in obj:
                return default_value
            obj = obj[key]
        return obj

    def __str__(self):
        return self.__dict__.__str__()

    def __unicode__(self):
        return self.__dict__.__unicode__()


ItemRange = namedtuple('ItemRange', 'start, end, total')


class PagedResult(Result):
    def __init__(self, response, callback, callback_kwargs, page_size=20):
        super(PagedResult, self).__init__(response)
        # range headers
        if 'item-range' in response.headers:
            if response.headers['item-range'] == "items */0":
                self.item_range = ItemRange(start=0, end=0, total=0)
            else:
                # in the format `items 0-9/100`
                items_part = response.headers['item-range'].split(' ')[1]
                range_part, total_part = items_part.split('/')
                start_part, end_part = range_part.split('-')
                self.item_range = ItemRange(start=int(start_part), end=int(end_part), total=int(total_part))

        self.callback = callback
        self.callback_kwargs = callback_kwargs
        self.page_size = page_size
        self._page = self.data.__iter__()

    def __iter__(self):
        return self

    def increment_page(self):
        if not hasattr(self, 'item_range'):
            raise StopIteration

        next_start = self.item_range.end + 1
        next_end = min(self.item_range.end + self.page_size, self.item_range.total)

        if next_start >= self.item_range.total:
            raise StopIteration

        self.callback_kwargs.update(start=next_start, end=next_end)

        next_page = self.callback(**self.callback_kwargs)
        self.item_range = next_page.item_range
        self._page = next_page.data.__iter__()

    def __next__(self):
        return self.next()

    def next(self):
        try:
            return next(self._page)
        except StopIteration:
            self.increment_page()
            return next(self._page)
